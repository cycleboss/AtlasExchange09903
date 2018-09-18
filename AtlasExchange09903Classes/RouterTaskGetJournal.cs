using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Globalization;

namespace AtlasExchangePlusClasses
{
    class RouterTaskGetJournal : RouterTask
    {
        protected List<JournalDataRow> journal;
        protected Dictionary<string, string> columnAliases;

        public RouterTaskGetJournal(UInt32 routerId, string journalId, Dictionary<string, string> columnAliases)
        {
            tag = "get_journal";
            RouterId = routerId;
            attributes["id"] = journalId;
            journal = new List<JournalDataRow>();
            this.columnAliases = columnAliases;
        }

        protected override void loadParameters()
        {
            var data = Database.GetData("select date_format(time, '%Y%m%d%H%i%s') from syncronize_data_time where router = " + RouterId +
                " and data = 'journal_" + attributes["id"] + "'");
            if (data != null && data.Length > 0 && data[0] != null && data[0].Length > 0)
            {
                attributes["min_time_stamp"] = data[0][0];
            }
        }

        protected override void parseResponseBody(XmlElement root)
        {
            var rowNode = root.FirstChild;
            while (rowNode != null)
            {
                if (rowNode.Name == "row")
                {
                    var attrs = rowNode.Attributes;
                    var values = new Dictionary<string, string>();
                    foreach (var column in columnAliases.Keys)
                    {
                        if (attrs[columnAliases[column]] != null)
                        {
                            values[column] = attrs[columnAliases[column]].Value;
                        }
                    }
                    var meterId = UInt32.Parse(attrs["id"].Value);
                    var dateTime = DateTime.ParseExact(attrs["t"].Value, "yyyyMMddHHmmss", CultureInfo.InvariantCulture);
                    var timeStamp = DateTime.ParseExact(attrs["ts"].Value, "yyyyMMddHHmmss", CultureInfo.InvariantCulture);
                    journal.Add(new JournalDataRow(meterId, dateTime, timeStamp, Database.GetMeteringPointId(meterId, dateTime.Date), values));
                }
                rowNode = rowNode.NextSibling;
            }
            Database.ClearMeteringPointsCache();
        }

        protected override void saveResult()
        {
            if (journal.Count < 1)
            {
                return;
            }
            var maxTimeStamp = DateTime.MinValue;
            var sql = "";
            var columnNames = columnAliases.Keys.ToList();
            var upd = "";
            foreach (var cn in columnNames)
            {
                upd += "," + cn + "=if(isnull(values(" + cn + "))," + cn + ",values(" + cn + "))";
            }
            foreach (JournalDataRow row in journal)
            {
                if (row.TimeStamp > maxTimeStamp)
                {
                    maxTimeStamp = row.TimeStamp;
                }
                sql += (sql.Length > 0 ? "," : "") + row.GetQuery(columnNames);
                if (sql.Length >= 1000 || row == journal[journal.Count - 1])
                {

                    Database.ExecuteNonQuery("insert into journal_" + attributes["id"] + " (meter,time,metering_point," + String.Join(",", columnNames) + ") values " + sql +
                        " on duplicate key update metering_point = values(metering_point)" + upd);
                    sql = "";
                }
            }
            Database.ExecuteNonQuery("insert into syncronize_data_time(router, data, time) values(" + RouterId + ", 'journal_" + attributes["id"] +
                "', " + maxTimeStamp.ToString("yyyyMMddHHmmss") + ") on duplicate key update time=values(time)");
        }

        protected override RouterTaskResult checkValidResponse(XmlDocument response)
        {
            var id = response.DocumentElement.Attributes["id"];
            if (id == null)
            {
                throw new Exception("Attribute id in response '" + tag + "' not found");
            }
            if (attributes["id"] != id.Value)
            {
                throw new Exception("Attribute id has unexpected value: '" + id.Value + "', excpected: '" + attributes["id"] + "'");
            }
            return base.checkValidResponse(response);
        }
    }
}
