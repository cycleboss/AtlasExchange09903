using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Globalization;

namespace AtlasExchangePlusClasses
{
    class JournalDataRow
    {
        public UInt32 Meter { get; private set; }
        public DateTime DateTime { get; private set; }
        public UInt32 MeteringPoint { get; set; }
        public DateTime TimeStamp { get; private set; }
        public Dictionary<string, string> Values { get; private set; }

        public JournalDataRow(UInt32 meter, DateTime dateTime, DateTime timeStamp, UInt32 meteringPoint = 0, Dictionary<string, string> values = null)
        {
            Meter = meter;
            DateTime = dateTime;
            TimeStamp = timeStamp;
            MeteringPoint = meteringPoint;
            Values = values == null ? new Dictionary<string, string>() : new Dictionary<string, string>(values);
        }

        public string GetQuery(List<string> columns)
        {
            var query = String.Format("{0:d},{1:yyyyMMddHHmmss},{2:s}", Meter, DateTime, MeteringPoint > 0 ? MeteringPoint.ToString() : "null");
            foreach (var col in columns)
            {
                query += "," + (Values.ContainsKey(col) ?  Values[col] : "null");
            }
            return "(" + query + ")";
        }

        public void SetValue(string name, string value)
        {
            Values[name] = value;
        }
    }
}
