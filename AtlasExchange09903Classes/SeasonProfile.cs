using System;
using System.Collections.Generic;
using System.Xml;
using System.Text;

namespace AtlasExchangePlusClasses
{
    class SeasonProfile : ConfigurableObject
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

        public string Date
        {
            get
            {
                return attributes["date"];
            }
            private set
            {
                attributes["date"] = value;
            }
        }

        public string WeeklyProfile
        {
            get
            {
                return attributes["weekly_profile"];
            }
            private set
            {
                attributes["weekly_profile"] = value;
            }
        }

        public SeasonProfile(string id, string date, string weeklyProfile)
        {
            Tag = "season_profile";
            Id = id;
            Date = date;
            WeeklyProfile = weeklyProfile;
        }
    }
}
