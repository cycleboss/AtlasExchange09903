using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AtlasExchangePlusTest
{
    class Test: AtlasExchangePlus.Service1
    {
        public void Start(string[] args)
        {
            OnStart(args);
        }

        public void Stop()
        {
            OnStop();
        }
    }
}
