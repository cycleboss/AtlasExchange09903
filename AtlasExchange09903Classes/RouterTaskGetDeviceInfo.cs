using System;
using System.Collections.Generic;
using System.Xml;
using System.Text;

namespace AtlasExchangePlusClasses
{
    class RouterTaskGetDeviceInfo : RouterTask
    {
        public string RouterNumber { get; private set; }
        public string RouterType { get; private set; }

        public RouterTaskGetDeviceInfo(UInt32 routerId)
        {
            tag = "get_device_info";
            RouterId = routerId;
        }

        protected override void parseResponseBody(XmlElement root)
        {
            RouterNumber = root.Attributes["number"].Value;
            RouterType = root.Attributes["type"].Value;
        }
    }
}
