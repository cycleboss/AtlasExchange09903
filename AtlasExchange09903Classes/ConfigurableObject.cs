using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace AtlasExchangePlusClasses
{
    class ConfigurableObject
    {
        protected Dictionary<string, string> attributes;
        public string Tag { get; protected set; }

        protected ConfigurableObject()
        {
            Tag = null;
            attributes = new Dictionary<string, string>();
        }
        /// <summary>
        /// Insert XML content of object inside node parent
        /// </summary>
        /// <param name="xmlDoc"></param>
        /// <param name="parent"></param>
        public void ToXml(XmlDocument xmlDoc, XmlElement parent)
        {
            var node = xmlDoc.CreateElement(Tag);
            if (attributes != null)
            {
                foreach (var attrKey in attributes.Keys)
                {
                    node.SetAttribute(attrKey, attributes[attrKey]);
                }
            }
            addInnerXml(xmlDoc, node);
            parent.AppendChild(node);
        }

        protected virtual void addInnerXml(XmlDocument xmlDoc, XmlElement parent)
        {
        }
    }
}
