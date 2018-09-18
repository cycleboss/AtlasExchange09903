using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AtlasExchangePlusClasses
{
    class AtlasExchangeException : Exception
    {
        public AtlasExchangeExceptionType Type { get; private set; }

        public AtlasExchangeException(AtlasExchangeExceptionType type, string message)
            : base(message)
        {
            Type = type;
        }
    }
}
