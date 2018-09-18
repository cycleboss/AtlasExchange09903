using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AtlasExchangePlusClasses
{
    abstract class TariffPlan : ConfigurableObject
    {
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

        public TariffPlanType Type
        {
            get
            {
                return (TariffPlanType)int.Parse(attributes["type"]);
            }
            private set
            {
                attributes["type"] = ((int)value).ToString();
            }
        }

        protected TariffPlan(string id, TariffPlanType type)
        {
            Tag = "tariff_plan";
            Id = id;
            Type = type;
        }

        public static TariffPlan Create(string id, TariffPlanType type)
        {
            TariffPlan tp;
            switch (type)
            {
                case TariffPlanType.Dlms:
                    {
                        tp = new TariffPlanDlms(id);
                        break;
                    }
                case TariffPlanType.RfPlc:
                    {
                        tp = new TariffPlanRfPlc(id);
                        break;
                    }
                default:
                    {
                        return null;
                    }
            }
            tp.load();
            return tp;
        }

        abstract protected void load();
        abstract public void Clear();
    }
}
