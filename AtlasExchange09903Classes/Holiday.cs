using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AtlasExchangePlusClasses
{
    class Holiday : ConfigurableObject
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

        public Holiday(string date)
        {
            Tag = "holiday";
            Date = date;
        }
    }
}
