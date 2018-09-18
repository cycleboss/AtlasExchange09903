using System;
using System.Collections.Generic;
using System.Xml;
using System.Text;

namespace AtlasExchangePlusClasses
{
    class RouterTaskSetConfigurationTemplates : RouterTask
    {
        private List<ConfigurationTemplate> configurationTemplates;

        public RouterTaskSetConfigurationTemplates(UInt32 routerId)
        {
            tag = "set_config_templates";
            RouterId = routerId;
            configurationTemplates = new List<ConfigurationTemplate>();
        }

        protected override void loadParameters()
        {
            var cts = new Dictionary<string, ConfigurationTemplate>();
            configurationTemplates.Clear();

            var data = Database.GetData("select config_template.id, config_template.title from config_template join meter on meter.config_template = config_template.id " +
                "join metering_point_meter on metering_point_meter.meter = meter.id and isnull(metering_point_meter.end_time) " +
                "join metering_point on metering_point.id = metering_point_meter.metering_point " +
                "where metering_point.router = " + RouterId);
            if (data == null || data.Length == 0)
            {
                return;
            }
            foreach (var ct in data)
            {
                cts[ct[0]] = new ConfigurationTemplate(ct[0], ct[1]);
                configurationTemplates.Add(cts[ct[0]]);
            }
            data = Database.GetData("select config_template, meter_param, value from config_template_value where config_template in (" + String.Join(",", cts.Keys) + ")");
            foreach (var param in data)
            {
                cts[param[0]].SetParameter(param[1], param[2]);
            }
        }

        protected override void createRequestBody(XmlDocument request)
        {
            var root = request.DocumentElement;
            foreach (var ct in configurationTemplates)
            {
                ct.ToXml(request, root);
            }
        }
    }
}
