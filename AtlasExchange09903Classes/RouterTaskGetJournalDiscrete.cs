using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AtlasExchangePlusClasses
{
    class RouterTaskGetJournalDiscrete : RouterTaskGetJournal
    {
        public RouterTaskGetJournalDiscrete(UInt32 routerId)
            : base(routerId, "discrete", new Dictionary<string, string>() 
            {
                { "event", "ev" },
                { "work_time", "wt" }
            })
        {
        }
    }
}
