using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading;

namespace AtlasExchangePlusClasses
{
    public static class Log
    {
        static ReaderWriterLock locker = new ReaderWriterLock();
        public static void Write(string log)
        {
            log = String.Format("{0:dd.MM.yyyy HH:mm:ss}: ", DateTime.Now) + log;
            Console.WriteLine(log);
            WriteFile(log);
        }

        public static void WriteFile(string log)
        {
            string fileName = AppDomain.CurrentDomain.BaseDirectory + DateTime.Now.ToString("yyyyMMdd") + ".log";
            try
            {
                locker.AcquireWriterLock(int.MaxValue);
                File.AppendAllLines(fileName, new String[] { log });
            }
            finally
            {
                locker.ReleaseWriterLock();
            }
        }
    }
}
