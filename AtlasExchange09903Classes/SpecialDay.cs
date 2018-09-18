using System;
using System.Collections.Generic;
using System.Xml;
using System.Text;

namespace AtlasExchangePlusClasses
{
    class SpecialDay : ConfigurableObject
    {
        public string Date
        {
            get
            {
                return attributes["date"];
            }

            private set
            {
                attributes["date"] = value;
            }
        }

        public byte DailyProfile
        {
            get
            {
                return Byte.Parse(attributes["daily_profile"]);
            }
            set
            {
                attributes["daily_profile"] = value.ToString();
            }
        }

        public SpecialDay(string date, byte dailyProfile)
        {
            Tag = "special_day";
            Date = date;
            DailyProfile = dailyProfile;
        }
    }
}
