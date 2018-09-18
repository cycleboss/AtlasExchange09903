using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AtlasExchangePlusClasses
{
    abstract class Router
    {
        public UInt32 Id { get; protected set; }
        public RouterType Type { get; protected set; } 
        public string Number { get; protected set; }
        public string Password { get; protected set; }
        public bool IsAuthorized { get; protected set; }
        public bool IsConnected { get { return connection.IsOpen; } }
        public bool CanConnect { get { return connection.CanOpen; } }
        protected IRouterConnection connection;

        protected void init(UInt32 id, RouterType type, string number, string password)
        {
            Id = id;
            Type = type;
            Number = number;
            Password = password;
            IsAuthorized = false;
        }

        public virtual bool Connect()
        {
            return connection.IsOpen ? true : connection.Open();
        }

        public bool Disconnect()
        {
            return connection.IsOpen ? connection.Close() : true;
        }

        public abstract void DoTask(RouterTask task);

        public abstract bool Login();
    }
}
