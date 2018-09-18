using System;
using System.Collections.Generic;
using System.Xml;
using System.Text;

namespace AtlasExchangePlusClasses
{
    interface IRouterTask09903
    {
        XmlDocument CreateRequest();

        XmlDocument ParseResponse(XmlDocument response);

        void Complete();
    }
}
