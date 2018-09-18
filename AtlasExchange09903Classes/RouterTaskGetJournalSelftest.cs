using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AtlasExchangePlusClasses
{
    class RouterTaskGetJournalSelftest : RouterTaskGetJournal
    {
        public RouterTaskGetJournalSelftest(UInt32 routerId)
            : base(routerId, "selftest", new Dictionary<string, string>() 
            {
                { "event", "ev" },
                { "work_time", "wt" }
            })
        {
        }
    }
}
