using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.IO;

using AtlasExchangePlusClasses;

namespace AtlasExchangePlus
{
    public partial class Service1 : ServiceBase
    {
        AtlasExchangePlusClasses.AtlasExchangePlus atlasExchange;
        public Service1()
        {
            InitializeComponent();
            var dbName = "webrms_extern_data";
            var userName = "root";
            var password = "";
            if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "services.ini"))
            {
                var opts = File.ReadAllLines(AppDomain.CurrentDomain.BaseDirectory + "services.ini");
                foreach (var opt in opts)
                {
                    if (opt == null)
                    {
                        continue;
                    }
                    if (opt.IndexOf("database=") == 0)
                    {
                        dbName = opt.Substring("database=".Length);
                    }
                    else if (opt.IndexOf("user=") == 0)
                    {
                        userName = opt.Substring("user=".Length);
                    }
                    else if (opt.IndexOf("password=") == 0)
                    {
                        password = opt.Substring("password=".Length);
                    }
                }
            }
            atlasExchange = new AtlasExchangePlusClasses.AtlasExchangePlus("mysql", "Database=" + dbName + ";Data Source=127.0.0.1;User Id=" + userName + 
                ";Password=" + password + "; CharSet=utf8");
        }

        protected override void OnStart(string[] args)
        {
            Log.Write("Service started with " + (args == null ? 0 : args.Length) + " parameters");
            atlasExchange.Start();
        }

        protected override void OnStop()
        {
            atlasExchange.Stop();
            Log.Write("Service stopped");
        }
    }
}
