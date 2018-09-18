using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AtlasExchangePlusClasses
{
    interface IRouterConnection
    {
        bool IsOpen { get; }
        bool CanOpen { get; }

        bool Open();
        bool Close();

        void Send(string msg);
        string Receive();
    }
}
