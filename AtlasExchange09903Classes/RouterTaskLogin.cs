using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AtlasExchangePlusClasses
{
    class RouterTaskLogin : RouterTask
    { 
        public RouterTaskLogin(UInt32 routerId)
        {
            tag = "login";
            RouterId = routerId;
        }

        protected override void loadParameters()
        {
            var data = Database.GetData("select password from router where id = " + RouterId);
            attributes["password"] = data[0][0];
        }
    }
}
