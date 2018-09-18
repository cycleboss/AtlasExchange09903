using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AtlasExchangePlusClasses
{
    class RouterTaskGetJournalOnOff : RouterTaskGetJournal
    {
        public RouterTaskGetJournalOnOff(UInt32 routerId) 
            : base(routerId, "on_off", new Dictionary<string,string>()
            {
                { "event", "ev" },
                { "work_time", "wt" },
            })
        {
        }
    }
}
