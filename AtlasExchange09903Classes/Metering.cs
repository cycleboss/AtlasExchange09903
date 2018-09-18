using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AtlasExchangePlusClasses
{
    class Metering
    {
        public UInt32 Meter { get; private set; }
        public DateTime DateTime { get; private set; }
        public byte Type { get; private set; }
        public string Value { get; private set; }
        public DateTime TimeStamp { get; private set; }
        public UInt32 MeteringPoint { get; set; }
        public Metering(UInt32 meter, DateTime dateTime, byte type, string value, DateTime timeStamp, UInt32 meteringPoint)
        {
            Meter = meter;
            DateTime = dateTime;
            Type = type;
            Value = value;
            TimeStamp = timeStamp;
            MeteringPoint = meteringPoint;
        }
    }
}
