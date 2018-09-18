using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AtlasExchangePlusClasses
{
    class RouterTaskGetJournalExternalInfluence : RouterTaskGetJournal
    {
        public RouterTaskGetJournalExternalInfluence(UInt32 routerId)
            : base(routerId, "external_influence", new Dictionary<string, string>()
            {
                { "event", "ev" },
                { "work_time", "wt" }
            })
        {
        }
    }
}
