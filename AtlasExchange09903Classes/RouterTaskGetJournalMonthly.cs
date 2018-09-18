using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AtlasExchangePlusClasses
{
    class RouterTaskGetJournalMonthly : RouterTaskGetJournal
    {
        public RouterTaskGetJournalMonthly(UInt32 routerId)
            : base(routerId, "monthly", new Dictionary<string, string>() 
            {
                { "tariff_1", "t1" },
                { "tariff_2", "t2" },
                { "tariff_3", "t3" },
                { "tariff_4", "t4" },
                { "tariff_5", "t5" },
                { "tariff_6", "t6" },
                { "tariff_7", "t7" },
                { "tariff_8", "t8" },
                { "active_forward", "ai" },
                { "active_reverse", "ae" },
                { "reactive_forward", "ri" },
                { "reactive_reverse", "re" },
                { "apparent_energy", "pe" },
                { "peak_power", "pw" },
                { "peak_power_time", "pt" },
                { "energy_losses", "el" },
                { "voltage_squared_hours", "vs" },
                { "work_time", "wt" } 
            })
        {
        }
    }
}
