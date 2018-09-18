using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Text;

namespace AtlasExchangePlusClasses
{
    
    class RouterTaskGetCoordinates: RouterTask
    {
        Dictionary<UInt32, List<String>> coordinates;
        public RouterTaskGetCoordinates(UInt32 routerId)
        {
            tag = "get_coordinates";
            RouterId = routerId;
            coordinates = new Dictionary<UInt32, List<String>>();
        }

        protected override void parseResponseBody(XmlElement root)
        {
            try
            {
                var coords = root.FirstChild;
                var now = DateTime.Now.Date;
                while (coords != null)
                {
                    if (coords.Name == "coordinates")
                    {
                        var attrs = coords.Attributes;
                        var meterId = UInt32.Parse(attrs["meter"].Value);
                        var latitude = attrs["lt"].Value;
                        var longitude = attrs["ln"].Value;
                        coordinates[Database.GetMeteringPointId(UInt32.Parse(attrs["meter"].Value), now)] = new List<String>() { attrs["lt"].Value, attrs["ln"].Value };
                    }
                    coords = coords.NextSibling;
                }
                Database.ClearMeteringPointsCache();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        protected override void saveResult()
        {
            try
            {
                var sql = "";
                foreach (var mp in coordinates.Keys)
                {
                    sql += (sql.Length == 0 ? "" : ",") + "(" + mp + ", " + coordinates[mp][0] + ", " + coordinates[mp][1] + ")";
                    if (sql.Length > 1000)
                    {
                        try
                        {
                            Database.ExecuteNonQuery("insert into metering_point(id, latitude, longitude) values " + sql + " on duplicate key update latitude = values(latitude), longitude = values(longitude)");
                            sql = "";
                        }
                        catch (Exception ex1)
                        {
                            Console.WriteLine(ex1.Message);
                        }
                    }
                }
                if (sql != "")
                {
                    Database.ExecuteNonQuery("insert into metering_point(id, latitude, longitude) values " + sql + " on duplicate key update latitude = values(latitude), longitude = values(longitude)");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
