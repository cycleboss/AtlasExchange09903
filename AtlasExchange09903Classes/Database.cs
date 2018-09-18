using System;
using System.Collections.Generic;
using System.Collections;
using System.Globalization;
using System.Text;
using System.Threading;
using System.Data.Common;
using MySql.Data.MySqlClient;

namespace AtlasExchangePlusClasses
{
    static class Database
    {
        private static volatile Dictionary<Thread, DbCommand> commands = new Dictionary<Thread, DbCommand>();
        private static Dictionary<Thread, Dictionary<UInt32, Dictionary<DateTime, UInt32>>> meteringPoints =
            new Dictionary<Thread, Dictionary<UInt32, Dictionary<DateTime, UInt32>>>();
 
        public static  string Type { get; private set; }
        
        public static string ConnectionString { get; private set; }
        
        public static DbCommand Command 
        { 
            get
            {
                var currentThread = Thread.CurrentThread;
                if (!commands.ContainsKey(currentThread))
                {
                    commands[currentThread] = getCommand();
                }
                return commands[currentThread];
            } 
        }

        public static void Init(string type, string connectionString)
        {
            Type = type;
            ConnectionString = connectionString;
        }

        public static DbDataReader GetReader(string sql)
        {
            Command.CommandText = sql;
            //if (Command.Connection.State == System.Data.ConnectionState.Closed)
            {
                Command.Connection.Open();
            }
            
            DbDataReader reader;
            try
            {
                reader = Command.ExecuteReader();
            }
            catch (Exception ex)
            {
                Command.Connection.Close();
                throw new AtlasExchangeException(AtlasExchangeExceptionType.MySqlException, ex.StackTrace + "(" + ex.Message + ")");
            }
            return reader;
        }

        public static void FreeReader(DbDataReader reader)
        {
            reader.Close();
            Command.Connection.Close();
        }

        public static int ExecuteNonQuery(string sql)
        {
            Command.CommandText = sql;
            Command.Connection.Open();
            int ret;
            try
            {
                ret = Command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Command.Connection.Close();
                throw new AtlasExchangeException(AtlasExchangeExceptionType.MySqlException, ex.StackTrace);
            }
            Command.Connection.Close();
            return ret;
        }

        public static void FreeCommand()
        {
            var currentThread = Thread.CurrentThread;
            if (commands.ContainsKey(currentThread))
            {
                commands.Remove(currentThread);
            }
        }

        public static string[][] GetData(string query)
        {
            var reader = GetReader(query);
            if (reader == null)
            {
                return null;
            }
            var ret = new ArrayList();
            while (reader.Read())
            {
                var row = new string[reader.FieldCount];
                for (var i = 0; i < row.Length; ++i)
                {
                    if (!reader.IsDBNull(i))
                    {
                        row[i] = reader.GetString(i);
                    }
                    else
                    {
                        row[i] = null;
                    }
                }
                ret.Add(row);
            }
            FreeReader(reader);
            return (string[][])ret.ToArray(typeof(string[]));
        }

        public static UInt32 GetMeteringPointId(UInt32 meter, DateTime dateTime)
        {
            var currentThread = Thread.CurrentThread;
            if (!meteringPoints.ContainsKey(currentThread))
            {
                meteringPoints[currentThread] = new Dictionary<UInt32, Dictionary<DateTime, UInt32>>();
            }
            if (!meteringPoints[currentThread].ContainsKey(meter))
            {
                meteringPoints[currentThread][meter] = new Dictionary<DateTime, UInt32>();
            }

            if (!meteringPoints[currentThread][meter].ContainsKey(dateTime))
            {
                var dateTimeStr = dateTime.ToString("yyyyMMddHHmmss");
                var data = GetData("select metering_point from metering_point_meter where meter = " + meter +
                " and time <= " + dateTimeStr + " and (end_time > " + dateTimeStr + " or isnull(end_time)) limit 1");
                meteringPoints[currentThread][meter][dateTime] = data != null && data.Length > 0 && data[0] != null && data[0].Length > 0 ? UInt32.Parse(data[0][0]): 0;
            }
            return meteringPoints[currentThread][meter][dateTime];
        }

        public static void SetConsumptionMeteringData(BoundaryTimeMetering[] boundaryTimeMeterings)
        {
            if (boundaryTimeMeterings == null)
            {
                return;
            }
            var exists = new Dictionary<UInt32, List<DateTime>>();
            foreach (var btm in boundaryTimeMeterings)
            {
                if (!exists.ContainsKey(btm.MeteringPoint))
                {
                    exists[btm.MeteringPoint] = new List<DateTime>();
                }
                var data = GetData("select date_format(max(time), '%Y%m%d%H%i%s') from metering where metering_point = " + 
                    btm.MeteringPoint +  " and type = " + btm.Type + " and time < " + btm.MinTime.ToString("yyyyMMddHHmmss") + "");
                if (data != null && data.Length > 0 && data[0] != null & data[0].Length > 0 && data[0][0] != null)
                {
                    var minDateTime = DateTime.ParseExact(data[0][0], "yyyyMMddHHmmss", CultureInfo.InvariantCulture);
                    for (var d = minDateTime.Date.AddDays(1); d <= btm.MinTime.Date; d = d.AddDays(1))
                    {
                        if (!exists[btm.MeteringPoint].Contains(d.Date))
                        {
                            exists[btm.MeteringPoint].Add(d.Date);
                        }
                    }
                    
                }
                else if (!exists[btm.MeteringPoint].Contains(btm.Date))
                {
                    exists[btm.MeteringPoint].Add(btm.Date);
                }
                data = GetData("select date_format(min(time), '%Y%m%d%H%i%s') from metering where metering_point = " +
                    btm.MeteringPoint + " and type = " + btm.Type + " and time > " + btm.MaxTime.ToString("yyyyMMddHHmmss") + "");
                if (data != null && data.Length > 0 && data[0] != null && data[0].Length > 0 && data[0][0] != null)
                {
                    var maxDateTime = DateTime.ParseExact(data[0][0], "yyyyMMddHHmmss", CultureInfo.InvariantCulture);
                    for (var d = btm.MaxTime.Date.AddDays(1); d <= maxDateTime.Date; d = d.AddDays(1))
                    {
                        if (!exists[btm.MeteringPoint].Contains(d.Date))
                        {
                            exists[btm.MeteringPoint].Add(d.Date);
                        }
                    }                   
                }
                else if (!exists[btm.MeteringPoint].Contains(btm.Date))
                {
                    exists[btm.MeteringPoint].Add(btm.Date);
                }
            }
            var sql = "";
            var now = DateTime.Now.ToString("yyyyMMddHHmmss");
            var cnt = 0;
            foreach (var mp in exists.Keys)
            {
                ++cnt;
                foreach (var date in exists[mp])
                {
                    sql += (sql.Length > 0 ? "," : "") + String.Format("({0:d},{1:yyyyMMdd},{2:s})", mp, date, now);
                    if (sql.Length > 1000 || (cnt == exists.Keys.Count && date == exists[mp][exists[mp].Count - 1] && sql.Length > 0))
                    {
                        ExecuteNonQuery("insert into consumption_metering_data(metering_point, date, created) values " + sql +
                            " on duplicate key update created = values(created)");
                        sql = "";
                    }
                }
            }
        }

        public static void ClearMeteringPointsCache()
        {
            meteringPoints.Remove(Thread.CurrentThread);
        }

        private static DbCommand getCommand()
        {
            switch (Type)
            {
                case "mysql":
                    {
                        return new MySqlCommand() { Connection = new MySqlConnection(ConnectionString), CommandTimeout = 120 };
                    }
                default:
                    {
                        return null;
                    }
            }
        }
    }
}
