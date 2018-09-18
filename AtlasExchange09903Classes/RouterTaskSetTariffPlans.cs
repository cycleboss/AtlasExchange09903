using System;
using System.Collections.Generic;
using System.Xml;
using System.Text;

namespace AtlasExchangePlusClasses
{
    class RouterTaskSetTariffPlans : RouterTask
    {
        private List<TariffPlan> tariffPlans;
        public RouterTaskSetTariffPlans(UInt32 routerId)
        {
            tag = "set_tariff_plans";
            RouterId = routerId;
            tariffPlans = new List<TariffPlan>();
        }

        protected override void loadParameters()
        {
            //var tps = new Dictionary<string, TariffPlan>();
            var data = Database.GetData("select tariff_plan.id, tariff_plan.type from tariff_plan join meter on meter.tariff_plan = tariff_plan.id " +
                "join metering_point_meter on metering_point_meter.meter = meter.id and isnull(metering_point_meter.end_time) " +
                "join metering_point on metering_point.id = metering_point_meter.metering_point " +
                "where metering_point.router = " + RouterId);
            if (data == null)
            {
                return;
            }
            foreach (var tp in data)
            {
                tariffPlans.Add(TariffPlan.Create(tp[0], (TariffPlanType)int.Parse(tp[1])));
            }
        }

        protected override void createRequestBody(XmlDocument request)
        {
            var root = request.DocumentElement;
            foreach (var tp in tariffPlans)
            {
                tp.ToXml(request, root);
            }
        }
    }
}
