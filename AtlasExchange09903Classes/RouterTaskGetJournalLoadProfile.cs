using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AtlasExchangePlusClasses
{
    class RouterTaskGetJournalLoadProfile : RouterTaskGetJournal
    {
        public RouterTaskGetJournalLoadProfile(UInt32 routerId)
            : base(routerId, "load_profile", new Dictionary<string, string>()
            { 
                { "active_forward", "ai" },
                { "active_reverse", "ae" },
                { "reactive_forward", "ri" },
                { "reactive_reverse", "re" },
                { "voltage", "v" },
                { "voltage_a", "va" },
                { "voltage_b", "vb" },
                { "voltage_c", "vc" },
                { "temperature", "tp" },
                { "duration", "dr" }
            })
        {
        }
    }
}
