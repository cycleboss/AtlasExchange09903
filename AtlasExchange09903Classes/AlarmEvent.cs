using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AtlasExchangePlusClasses
{
    class AlarmEvent : ConfigurableObject
    {
        public string Id
        {
            get
            {
                return attributes["id"];
            }
            private set
            {
                attributes["id"] = value;
            }
        }

        public AlarmEvent(string id)
        {
            Tag = "event";
            Id = id;
        }
    }
}
