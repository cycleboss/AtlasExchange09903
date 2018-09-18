using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AtlasExchangePlusClasses
{
    class RouterTaskGetJournalCurrent : RouterTaskGetJournal
    {
        public RouterTaskGetJournalCurrent(UInt32 routerId)
            : base(routerId, "current", new Dictionary<string, string>() 
            {
                { "event", "ev" },
                { "work_time", "wt" }
            })
        {
        }
    }
}
