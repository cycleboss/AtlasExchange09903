using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AtlasExchangePlusClasses
{
    class RouterTaskGetJournalEnergyQuality : RouterTaskGetJournal
    {
        public RouterTaskGetJournalEnergyQuality(UInt32 routerId)
            : base(routerId, "energy_quality", new Dictionary<string, string>()
            {
                { "event", "ev" },
                { "work_time", "wt" }
            })
        {
        }
    }
}
