using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AtlasExchangePlusClasses
{
    class WeeklyProfile : ConfigurableObject
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

        public byte Monday
        {
            get
            {
                return Byte.Parse(attributes["monday"]);
            }
            private set
            {
                attributes["monday"] = value.ToString();
            }
        }

        public byte Tuesday
        {
            get
            {
                return Byte.Parse(attributes["tuesday"]);
            }
            private set
            {
                attributes["tuesday"] = value.ToString();
            }
        }

        public byte Wednesday
        {
            get
            {
                return Byte.Parse(attributes["wednesday"]);
            }
            private set
            {
                attributes["wednesday"] = value.ToString();
            }
        }

        public byte Thursday
        {
            get
            {
                return Byte.Parse(attributes["thursday"]);
            }
            private set
            {
                attributes["thursday"] = value.ToString();
            }
        }

        public byte Friday
        {
            get
            {
                return Byte.Parse(attributes["friday"]);
            }
            private set
            {
                attributes["friday"] = value.ToString();
            }
        }

        public byte Saturday
        {
            get
            {
                return Byte.Parse(attributes["saturday"]);
            }
            private set
            {
                attributes["saturday"] = value.ToString();
            }
        }

        public byte Sunday
        {
            get
            {
                return Byte.Parse(attributes["sunday"]);
            }
            private set
            {
                attributes["sunday"] = value.ToString();
            }
        }

        public WeeklyProfile(string id, byte monday, byte tuesday, byte wednesday, byte thursday, byte friday, byte saturday, byte sunday)
        {
            Tag = "weekly_profile";
            Id = id;
            Monday = monday;
            Tuesday = tuesday;
            Wednesday = wednesday;
            Thursday = thursday;
            Friday = friday;
            Saturday = saturday;
            Sunday = sunday;
        }
    }
}
