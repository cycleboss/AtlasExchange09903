using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AtlasExchangePlusClasses
{
    enum RouterTaskResult
    {
        Success = 1,
        AuthenticationFailed = 2,
        UnknownCommand = 3,
        RuntimeError = 4
    }
}
