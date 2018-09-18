using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AtlasExchangePlusClasses
{
    class TaskStatus : ConfigurableObject
    {
        public UInt32 Task
        {
            get;
            private set;
        }

        public UInt32 Meter
        {
            get;
            private set;
        }

        public DateTime Time
        {
            get;
            private set;
        }

        public byte Result
        {
            get;
            private set;
        }

        public UInt32 MeteringPoint
        {
            get;
            private set;
        }

        public TaskStatus(UInt32 task, UInt32 meter, DateTime time, byte result, UInt32 meteringPoint)
        {
            Task = task;
            Meter = meter;
            Time = time;
            Result = result;
            MeteringPoint = meteringPoint;
        }
    }
}
