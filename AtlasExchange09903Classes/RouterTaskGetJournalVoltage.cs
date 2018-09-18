using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AtlasExchangePlusClasses
{
    class RouterTaskGetJournalVoltage : RouterTaskGetJournal
    {
        public RouterTaskGetJournalVoltage(UInt32 routerId)
            : base(routerId, "voltage", new Dictionary<string, string>() 
            {
                { "event", "ev" },
                { "voltage", "v" },
                { "depth", "dp" },
                { "duration","dr" },
                { "work_time", "wt" }
            })
        {
        }
        

    }
}
