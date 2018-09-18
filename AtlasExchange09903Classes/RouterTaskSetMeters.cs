using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace AtlasExchangePlusClasses
{
    class RouterTaskSetMeters : RouterTask
    {
        private List<Meter> meters;

        public RouterTaskSetMeters(UInt32 routerId)
        {
            tag = "set_meters";
            RouterId = routerId;
        }

        protected override void loadParameters()
        {
            meters = new List<Meter>();
            var metersArray = Database.GetData("select meter.id, meter.number, meter.type, meter.interface, meter.protocol, meter.password, metering_point.plc_group, meter.address, " + 
                "meter.config_template, meter.tariff_plan, metering_point.latitude, metering_point.longitude from meter join metering_point_meter on meter.id = metering_point_meter.meter " +
                "join metering_point on metering_point_meter.metering_point =  metering_point.id " + 
                "where metering_point.router = " + RouterId + " and isnull(metering_point_meter.end_time)");
            foreach (var meter in metersArray)
            {
                this.meters.Add(new Meter(meter[0], meter[1], meter[2], meter[3], meter[4], meter[5], (meter[3] == "4" || meter[3] == "5") ? meter[6] : meter[7], meter[8], meter[9], meter[10], meter[11]));
            }
        }

        protected override void createRequestBody(XmlDocument request)
        {
            var root = request.DocumentElement;
            foreach (var meter in meters)
            {
                meter.ToXml(request, root);
            }
            request.AppendChild(root);
        }
    }
}
