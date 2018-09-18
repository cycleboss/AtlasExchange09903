using System;
using System.Collections.Generic;
using System.Xml;
using System.Text;
using System.Globalization;

namespace AtlasExchangePlusClasses
{
    class TariffPlanRfPlc : TariffPlan
    {
        private List<TariffZone> tariffZones;
        private List<Holiday> holidays;
        private List<MovedDay> movedDays;

        public TariffPlanRfPlc(string id)
            : base(id, TariffPlanType.RfPlc)
        {
            tariffZones = new List<TariffZone>();
            holidays = new List<Holiday>();
            movedDays = new List<MovedDay>();
        }

        protected override void load()
        {
            Clear();

            var data = Database.GetData("select date_format(start_time, '%H%i'), tariff, week_mask, month_mask from rim_tariff_zone where tariff_plan = " + Id);
            foreach (var tz in data)
            {
                //var time = DateTime.Now.Date.AddMinutes(int.Parse(tz[0])).ToString("HHmm");
                tariffZones.Add(new TariffZone(tz[0], Byte.Parse(tz[1]), Byte.Parse(tz[2]), UInt16.Parse(tz[3])));
            }

            data = Database.GetData("select month, day from rim_tariff_holidays where tariff_plan = " + Id);
            foreach (var h in data)
            {
                var date = new DateTime(DateTime.Now.Year, int.Parse(h[0]), int.Parse(h[1])).ToString("MMdd");
                holidays.Add(new Holiday(date));
            }

            data = Database.GetData("select date_format(date, '%Y%m%d'), week_day from rim_tariff_moved_days where tariff_plan = " + Id);
            foreach (var md in data)
            {
                movedDays.Add(new MovedDay(DateTime.ParseExact(md[0], "yyyyMMdd", CultureInfo.InvariantCulture), Byte.Parse(md[1])));
            }
        }

        public override void Clear()
        {
            tariffZones.Clear();
            holidays.Clear();
            movedDays.Clear();
        }

        protected override void addInnerXml(XmlDocument xmlDoc, XmlElement parent)
        {
            foreach (var tz in tariffZones)
            {
                tz.ToXml(xmlDoc, parent);
            }

            foreach (var h in holidays)
            {
                h.ToXml(xmlDoc, parent);
            }

            foreach (var md in movedDays)
            {
                md.ToXml(xmlDoc, parent);
            }
        }
    }
}
