using System;
using System.Collections.Generic;
using System.Xml;
using System.Text;
using System.Linq;

namespace AtlasExchangePlusClasses
{
    class RouterTaskGetJournalDaily : RouterTaskGetJournal
    {
        public RouterTaskGetJournalDaily(UInt32 routerId)
            : base(routerId, "daily", new Dictionary<string, string>()
            { 
                { "tariff_1", "t1" },
                { "tariff_2", "t2" },
                { "tariff_3", "t3" },
                { "tariff_4", "t4" },
                { "tariff_5", "t5" },
                { "tariff_6", "t6" },
                { "tariff_7", "t7" },
                { "tariff_8", "t8" },
                { "active_forward", "ai" },
                { "active_reverse", "ae" },
                { "reactive_forward", "ri" },
                { "reactive_reverse", "re" },
                { "energy_losses", "el" },
                { "voltage_squared_hours", "vs" },
                { "fault_frequency_time", "ft" },
                { "fault_energy_status", "fs" },
                { "work_time", "wt" }            
            })
        {
        }

        protected override void saveResult()
        {
            base.saveResult();
            var sql = "";
            byte mt;
            foreach (var row in journal)
            {
                foreach (var mType in row.Values.Keys)
                {
                    try
                    {
                        mt = getMeteringCode(mType);
                    }
                    catch (AtlasExchangeException ex)
                    {
                        Log.Write(ex.Message);
                        continue;
                    }
                    sql += (sql.Length > 0 ? "," : "") + "(" + row.Meter + ", " + row.DateTime.ToString("yyyyMMddHHmmss") + ", " + row.MeteringPoint + ", " + "null"
                        /*row.TimeStamp.ToString("yyyyMMddHHmmss")*/ + ", " + mt + "," + row.Values[mType] + ")";
                }

                if (sql.Length >= 1000 || (sql.Length > 0 && row == journal.Last()))
                {
                    Database.ExecuteNonQuery("insert into metering(meter, time, metering_point, time_stamp, type, value) values " + sql + 
                        " on duplicate key update metering_point = values(metering_point), value = values(value)");
                    sql = "";
                }
            }
            Database.SetConsumptionMeteringData(getBoundaryTimeMeterings());
        }

        private BoundaryTimeMetering[] getBoundaryTimeMeterings()
        {
            var boundaryTimeMeterings = new List<BoundaryTimeMetering>();
            byte mt = 0;
            foreach (var row in journal)
            {
                foreach (var mType in row.Values.Keys)
                {
                    try
                    {
                        mt = getMeteringCode(mType);
                    }
                    catch (AtlasExchangeException ex)
                    {
                        Log.Write(ex.Message);
                        continue;
                    }
                    boundaryTimeMeterings.Add(new BoundaryTimeMetering(row.MeteringPoint, row.DateTime.Date, mt, row.DateTime, row.DateTime));
                }
            }
            return boundaryTimeMeterings.ToArray();
        }

        private static byte getMeteringCode(string mType)
        {
            switch (mType)
            {
                case "tariff_1":
                case "tariff_2":
                case "tariff_3":
                case "tariff_4":
                case "tariff_5":
                case "tariff_6":
                case "tariff_7":
                case "tariff_8":
                    {
                        return Byte.Parse(mType.Substring(7));
                    }
                case "active_forward":
                    {
                        return 64;
                    }
                case "active_reverse":
                    {
                        return 65;
                    }
                case "reactive_forward":
                    {
                        return 66;
                    }
                case "reactive_reverse":
                    {
                        return 69;
                    }
                default:
                    {
                        throw new AtlasExchangeException(AtlasExchangeExceptionType.UnknownMetering, "Unknown metering: " + mType);
                    }
            }
        }
    }
}
