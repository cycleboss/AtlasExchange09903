using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace AtlasExchangePlusClasses
{
    class RouterTaskSetAlarmEvents : RouterTask
    {
        private List<AlarmEvent> alarmEvents;

        public string Phone 
        {
            get
            {
                return attributes.ContainsKey("phone") ? attributes["phone"] : null;
            }
            private set
            {
                if (value != null)
                {
                    attributes["phone"] = value;
                }
            }
        }

        public string Ip
        {
            get
            {
                return attributes.ContainsKey("ip") ? attributes["ip"] : null;
            }
            private set
            {
                if (value != null)
                {
                    attributes["ip"] = value;
                }
            }
        }

        public string StartDate
        {
            get
            {
                return attributes.ContainsKey("start_date") ? attributes["start_date"] : null;
            }
            private set
            {
                if (value != null)
                {
                    attributes["start_date"] = value;
                }
            }
        }

        public RouterTaskSetAlarmEvents(UInt32 routerID, string phone = null, string ip = null, string startDate = null)
        {
            tag = "set_alarm_events";
            alarmEvents = new List<AlarmEvent>();
            RouterId = routerID;
            Phone = phone;
            Ip = ip;
            StartDate = startDate;
        }

        protected override void loadParameters()
        {
            //var data = Database.GetData("");
            alarmEvents.Add(new AlarmEvent("break_cleat"));
        }
    }
}
