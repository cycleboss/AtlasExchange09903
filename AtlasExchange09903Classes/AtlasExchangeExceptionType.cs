using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AtlasExchangePlusClasses
{
    enum AtlasExchangeExceptionType
    {
        MySqlException,
        ConnectionFail,
        WrongResponse,
        NoResponse,
        UnknownMetering,
        UnknownException,
    }
}
