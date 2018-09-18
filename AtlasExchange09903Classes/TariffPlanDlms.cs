using System;
using System.Collections.Generic;
using System.Xml;
using System.Text;

namespace AtlasExchangePlusClasses
{
    class TariffPlanDlms : TariffPlan
    {
        private List<DailyProfile> dailyProfiles;
        private List<WeeklyProfile> weeklyProfiles;
        private List<SeasonProfile> seasonProfiles;
        private List<SpecialDay> specialDays;

        public TariffPlanDlms(string id)
            : base(id, TariffPlanType.Dlms)
        {
            dailyProfiles = new List<DailyProfile>();
            weeklyProfiles = new List<WeeklyProfile>();
            seasonProfiles = new List<SeasonProfile>();
            specialDays = new List<SpecialDay>();
        }

        protected override void load()
        {
            Clear();
            var data = Database.GetData("select id, date_format(time, '%H%i'), tariff from dlms_tariff_daily_schedule where tariff_plan = " + Id);
            foreach (var dp in data)
            {
                dailyProfiles.Add(new DailyProfile(Byte.Parse(dp[0]), dp[1], Byte.Parse(dp[2])));
            }
            data = Database.GetData("select id, monday, tuesday, wednesday, thursday, friday, saturday, sunday from dlms_tariff_weekly_schedule where tariff_plan = " + Id);
            foreach (var wp in data)
            {
                weeklyProfiles.Add(new WeeklyProfile(wp[0], Byte.Parse(wp[1]), Byte.Parse(wp[2]), Byte.Parse(wp[3]), Byte.Parse(wp[4]), Byte.Parse(wp[5]), Byte.Parse(wp[6]), Byte.Parse(wp[7])));
            }

            data = Database.GetData("select id, date_format(date, '%m%d'), weekly_schedule from dlms_tariff_season_schedule where tariff_plan = " + Id);
            foreach (var sp in data)
            {
                seasonProfiles.Add(new SeasonProfile(sp[0], sp[1], sp[2]));
            }

            data = Database.GetData("select date_format(date, '%m%d'), day_id from dlms_tariff_special_days where tariff_plan = " + Id);
            foreach (var sp in data)
            {
                specialDays.Add(new SpecialDay(sp[0], Byte.Parse(sp[1])));
            }
        }


        protected override void addInnerXml(XmlDocument xmlDoc, XmlElement parent)
        {
            foreach (var dp in dailyProfiles)
            {
                dp.ToXml(xmlDoc, parent);
            }

            foreach (var wp in weeklyProfiles)
            {
                wp.ToXml(xmlDoc, parent);
            }

            foreach (var sp in seasonProfiles)
            {
                sp.ToXml(xmlDoc, parent);
            }

            foreach (var sd in specialDays)
            {
                sd.ToXml(xmlDoc, parent);
            }
        }

        public override void Clear()
        {
            dailyProfiles.Clear();
            weeklyProfiles.Clear();
            seasonProfiles.Clear();
            specialDays.Clear();
        }
    }
}
