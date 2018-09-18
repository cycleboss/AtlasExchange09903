using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AtlasExchangePlusClasses
{
    class RouterTaskGetJournalTg : RouterTaskGetJournal
    {
        public RouterTaskGetJournalTg(UInt32 routerId)
            : base(routerId, "tg", new Dictionary<string, string>() 
            {
                { "event", "ev" },
                { "work_time", "wt" }
            })
        {
        }
    }
}
