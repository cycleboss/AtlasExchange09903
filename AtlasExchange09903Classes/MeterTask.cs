using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace AtlasExchangePlusClasses
{
    class MeterTask : ConfigurableObject
    {
        private Dictionary<string, string> parameters;
        protected List<string> meters;

        public string Id
        {
            get
            {
                return attributes["id"];
            }
            private set
            {
                attributes["id"] = value;
            }
        }

        public string Type
        {
            get
            {
                return attributes["type"];
            }
            private set
            {
                attributes["type"] = value;
            }
        }

        public string PeriodType
        {
            get
            {
                return attributes["period_type"];
            }
            private set
            {
                attributes["period_type"] = value;
            }
        }

        public string Period
        {
            get
            {
                return attributes["period"];
            }
            private set
            {
                attributes["period"] = value;
            }
        }

        public string Priority
        {
            get
            {
                return attributes["proprity"];
            }
            private set
            {
                attributes["priority"] = value;
            }
        }

        public MeterTask(string id, string type, string periodType, string period, string priority, List<string> meters = null, Dictionary<string, string> parameters = null)
        {
            Tag = "task";
            Id = id;
            Type = type;
            PeriodType = periodType;
            Period = period;
            Priority = priority;
            this.parameters = parameters == null ? new Dictionary<string, string>() : new Dictionary<string, string>(parameters);
            this.meters = meters == null ? new List<string>() : new List<string>(meters);
        }

        public void AddParameter(string key, string value)
        {
            parameters[key] = value;
        }

        public void AddMeter(string meter)
        {
            meters.Add(meter);
        }

        protected override void addInnerXml(XmlDocument xmlDoc, XmlElement parent)
        {
            foreach (var paramKey in parameters.Keys)
            {
                if (!attributes.ContainsKey(paramKey))
                {
                    parent.SetAttribute(paramKey, parameters[paramKey]);
                }
            }
            foreach (var meter in meters)
            {
                var meterNode = xmlDoc.CreateElement("meter");
                meterNode.SetAttribute("id", meter);
                parent.AppendChild(meterNode);
            }
        }
    }
}
