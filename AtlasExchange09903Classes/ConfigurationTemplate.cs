using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AtlasExchangePlusClasses
{
    class ConfigurationTemplate : ConfigurableObject
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

        public string Title
        {
            get
            {
                return attributes["title"];
            }
            private set
            {
                attributes["title"] = value;
            }
        }

        public ConfigurationTemplate(string id, string title)
        {
            Tag = "config_template";
            Id = id;
            Title = title;
        }

        public void SetParameter(string name, string value)
        {
            attributes[name] = value;
        }
    }
}
