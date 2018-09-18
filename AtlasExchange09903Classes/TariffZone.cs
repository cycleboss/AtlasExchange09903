using System;
using System.Collections.Generic;
using System.Xml;
using System.Text;

namespace AtlasExchangePlusClasses
{
    class TariffZone : ConfigurableObject
    {
        public string Time 
        {
            get
            {
                return attributes["time"];
            }
            private set
            {
                attributes["time"] = value;
            }
        }

        public byte Tariff
        {
            get
            {
                return Byte.Parse(attributes["tariff"]);
            }
            private set
            {
                attributes["tariff"] = value.ToString();
            }
        }

        public byte WeekMask
        {
            get
            {
                var chars = attributes["week_mask"].ToCharArray();
                Array.Reverse(chars);
                return Convert.ToByte(new string(chars), 2); 
            }
            private set
            {
                var chars = Convert.ToString(value, 2).PadLeft(8, '0').ToCharArray();
                Array.Reverse(chars);
                attributes["week_mask"] = new string(chars);
            }
        }

        public UInt16 MonthMask
        {
            get
            {
                var chars = attributes["month_mask"].ToCharArray();
                Array.Reverse(chars);
                return (UInt16)(Convert.ToUInt16(new string(chars), 2) & 0x0FFF);
            }
            private set
            {
                var chars = Convert.ToString(value, 2).PadLeft(12, '0').ToCharArray();
                Array.Reverse(chars);
                attributes["month_mask"] = new string(chars);
            }
        }

        public TariffZone(string time, byte tariff, byte weekMask, UInt16 monthMask)
        {
            Tag = "tariff_zone";
            Time = time;
            Tariff = tariff;
            WeekMask = weekMask;
            MonthMask = monthMask;
        }
    }
}
