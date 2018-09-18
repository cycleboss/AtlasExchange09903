using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AtlasExchangePlusClasses
{
    class RouterTaskGetJournalCorrections : RouterTaskGetJournal
    {
        public RouterTaskGetJournalCorrections(UInt32 routerId)
            : base(routerId, "corrections", new Dictionary<string, string>() 
            {
                { "event", "ev" },
                { "interface", "if" },
                { "work_time", "wt" },
            })
        {
        }
    }
}
