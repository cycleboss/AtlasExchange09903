using System;
using System.Globalization;
using System.Collections.Generic;
using System.Xml;
using System.Linq;
using System.Text;

namespace AtlasExchangePlusClasses
{
    class RouterTaskGetCurrentMeterings : RouterTask
    {
        private List<Metering> meterings;
        private byte[] aeMeterings = new byte[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 64 };

        public RouterTaskGetCurrentMeterings(UInt32 routerId)
        {
            tag = "get_current_meterings";
            RouterId = routerId;
            meterings = new List<Metering>();
        }

        protected override void loadParameters()
        {
            var mintTimeStamp = Database.GetData("select date_format(max(time_stamp), '%Y%m%d%H%i%s') " +
                "from metering join metering_point on metering.metering_point = metering_point.id " +
                "where metering_point.router = " + RouterId);
            if (mintTimeStamp != null && mintTimeStamp.Length > 0 && mintTimeStamp[0] != null)
            {
                attributes["min_time_stamp"] = mintTimeStamp[0][0];
            }
        }

        protected override void parseResponseBody(XmlElement root)
        {
            var metering = root.FirstChild;
            while (metering != null)
            {
                if (metering.Name == "metering")
                {
                    var attrs = metering.Attributes;
                    var meterId = UInt32.Parse(attrs["id"].Value);
                    var dateTime = DateTime.ParseExact(attrs["t"].Value, "yyyyMMddHHmmss", CultureInfo.InvariantCulture);
                    meterings.Add(new Metering(meterId, dateTime, Byte.Parse(attrs["n"].Value), attrs["v"].Value,
                        DateTime.ParseExact(attrs["ts"].Value, "yyyyMMddHHmmss", CultureInfo.InvariantCulture), Database.GetMeteringPointId(meterId, dateTime)));
                }
                metering = metering.NextSibling;
            }
            Database.ClearMeteringPointsCache();
        }

        protected override void saveResult()
        {
            var sql = "";
            foreach (var metering in meterings)
            {
                sql += (sql.Length == 0 ? "" : ",") + String.Format("({0:d}, {1:yyyyMMddHHmmss}, {2:d}, {3:s}, {4:yyyyMMddHHmmss}, {5:s})",
                    metering.Meter, metering.DateTime, metering.Type, metering.Value, metering.TimeStamp, metering.MeteringPoint == 0 ? "NULL" : 
                    metering.MeteringPoint.ToString());
                if (sql.Length > 1000 || metering == meterings[meterings.Count - 1])
                {
                    Database.ExecuteNonQuery("insert into metering(meter, time, type, value, time_stamp, metering_point) values " + sql +
                    " on duplicate key update value = values(value), time_stamp = values(time_stamp)");
                    sql = "";
                }
            }
            Database.SetConsumptionMeteringData(getBoundaryTimeMeterings());
        }

        private BoundaryTimeMetering[] getBoundaryTimeMeterings()
        {
            return
                    (from metering in meterings
                    where ((metering.Type <= 8 || metering.Type == 64) && metering.MeteringPoint > 0)
                    group metering by new
                    {
                        metering.MeteringPoint,
                        metering.DateTime.Date,
                        metering.Type
                    }
                        into grouping
                        select new BoundaryTimeMetering(grouping.Key.MeteringPoint, grouping.Key.Date, grouping.Key.Type, 
                            grouping.Min(t => t.DateTime), grouping.Max(t => t.DateTime))).ToArray();
        }
    }
}
