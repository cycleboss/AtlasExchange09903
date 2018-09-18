using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace AtlasExchangePlusClasses
{
    class Meter : ConfigurableObject
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
        public string Number 
        {
            get 
            { 
                return attributes["number"];
            } 
            private set 
            { 
                attributes["number"] = value; 
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
        public string Interface 
        { 
            get 
            { 
                return attributes["interface"]; 
            }
            private set
            {
                attributes["interface"] = value;
            }
        }
        public string Protocol 
        { 
            get 
            { 
                return attributes["protocol"]; 
            }
            private set
            {
                attributes["protocol"] = value;
            }
        }
        public string Password 
        { 
            get 
            { 
                return attributes["password"]; 
            }
            private set
            {
                attributes["password"] = value;
            }
        }
        public string AddressGroup 
        { 
            get 
            { 
                return attributes.ContainsKey("address_group") ? attributes["address_group"] : null; 
            }
            set
            {
                attributes["address_group"] = value;
            }
        }
        public string Tariff 
        { 
            get 
            { 
                return attributes.ContainsKey("tariff") ? attributes["tariff"] : null; 
            }
            set
            {
                attributes["tariff"] = value;
            }
        }
        public string Config 
        { 
            get 
            {
                return attributes.ContainsKey("config") ? attributes["config"] : null; 
            }
            set
            {
                attributes["config"] = value;
            }
        }
        public string Latitude
        {
            get
            {
                return attributes.ContainsKey("latitude") ? attributes["latitude"] : null;
            }
            private set
            {
                if (value != null)
                {
                    attributes["latitude"] = value;
                }
            }
        }
        public string Longitude
        {
            get
            {
                return attributes.ContainsKey("longitude") ? attributes["longitude"] : null;
            }
            private set
            {
                if (value != null)
                {
                    attributes["longitude"] = value;
                }
            }
        }
        public Dictionary<string, string> Attributes
        {
            get
            {
                return new Dictionary<string, string>(attributes);
            }
        }

        public Meter(string id, string number, string type, string intface, string protocol, string password, string addressGroup, string config, string tariff, 
            string latitude, string longitude)
        {
            Tag = "meter";
            attributes = new Dictionary<string, string>();
            Id = id;
            Number = number;
            Type = type;
            Interface = intface;
            Protocol = protocol;
            Password = password;
            AddressGroup = addressGroup;
            Config = config;
            Tariff = tariff;
            Latitude = latitude;
            Longitude = longitude;
        }
    }
}
