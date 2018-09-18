using System;
using System.Collections.Generic;
using System.Xml;
using System.Text;

namespace AtlasExchangePlusClasses
{
    class DailyProfile : ConfigurableObject
    {
        public byte Id 
        {
            get
            {
                return Byte.Parse(attributes["id"]);
            }
            private set
            {
                attributes["id"] = value.ToString();
            } 
        }

        public string Time 
        {
            get
            {
                return attributes["time"];
            }
            private set
            {
                attributes["time"] = value;
            }
        }

        public byte Tariff 
        {
            get
            {
                return Byte.Parse(attributes["tariff"]);
            }
            private set
            {
                attributes["tariff"] = value.ToString();
            }
        }

        public DailyProfile(byte id, string time, byte tariff)
        {
            Tag = "daily_profile";
            Id = id;
            Time = time;
            Tariff = tariff;
        }
    }
}
