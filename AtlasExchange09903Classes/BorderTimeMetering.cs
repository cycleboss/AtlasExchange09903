using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AtlasExchangePlusClasses
{
    class BoundaryTimeMetering
    {
        public UInt32 MeteringPoint { get; private set; }
        public DateTime Date { get; private set; }
        public byte Type { get; private set; }
        public DateTime MinTime { get; private set; }
        public DateTime MaxTime { get; private set; }

        public BoundaryTimeMetering(UInt32 meteringPoint, DateTime date, byte type, DateTime minTime, DateTime maxTime)
        {
            MeteringPoint = meteringPoint;
            Date = date.Date;
            Type = type;
            MinTime = Date.Add(minTime.TimeOfDay);
            MaxTime = Date.Add(maxTime.TimeOfDay);
        }
    }
}
