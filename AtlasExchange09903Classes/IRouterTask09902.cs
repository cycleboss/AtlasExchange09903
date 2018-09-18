using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AtlasExchangePlusClasses
{
    interface IRouterTask09902
    {
        string[] CreateRequest();

        string[] ParseResponse(string request);

        void Complete();
    }
}
