using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace AtlasExchangePlusClasses
{
    class MovedDay : ConfigurableObject
    {
        public DateTime Date
        {
            get
            {
                return DateTime.ParseExact(attributes["date"], "yyyyMMdd", CultureInfo.InvariantCulture);
            }
            private set
            {
                attributes["date"] = value.ToString("yyyyMMdd");
            }
        }

        public byte WeekDay
        {
            get
            {
                return Byte.Parse(attributes["week_day"]);
            }
            private set
            {
                attributes["week_day"] = value.ToString();
            }
        }

        public MovedDay(DateTime date, byte weekDay)
        {
            Tag = "moved_day";
            Date = date;
            WeekDay = weekDay;
        }
    }
}
