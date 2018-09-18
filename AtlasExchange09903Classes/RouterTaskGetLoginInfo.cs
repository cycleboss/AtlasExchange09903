using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace AtlasExchangePlusClasses
{
    class RouterTaskGetLoginInfo : RouterTask
    {
        public string Protocol { get; private set; }
        public string Encryption { get; private set; }

        public RouterTaskGetLoginInfo()
        {
            tag = "get_login_info";
        }

        protected override void parseResponseBody(XmlElement root)
        {
            var rootAttrs = root.Attributes;
            Protocol = rootAttrs["version"].Value;
            Encryption = rootAttrs["encryption"] != null ? rootAttrs["encryption"].Value : null;
        }
    }
}
