using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AtlasExchangePlusClasses
{
    interface IDataProvider
    {
        List<Meter> GetMeters(UInt32 routerId);
        List<MeterTask> GetMetersTasks(UInt32 routerId);
        List<RouterTask> GetRouterTasks(UInt32 routerId);
        UInt32 GetMeteringPoint(UInt32 meterId, DateTime date);
        DateTime GetMaxTimeStamp(string meteringName);
        void SaveCurrentMeterings(List<Metering> meterings);
        void SaveJournalRows(string journalName, List<JournalDataRow> rows);
        
    }

    class MySQLDataProvider : IDataProvider
    {
        public List<Meter> GetMeters(UInt32 routerId)
        {
            return null;
        }

        public List<MeterTask> GetMetersTasks(UInt32 routerId)
        {
            return null;
        }

        public List<RouterTask> GetRouterTasks(UInt32 routerId)
        {
            return null;
        }

        public UInt32 GetMeteringPoint(UInt32 meterId, DateTime date)
        {
            return 0;
        }

        public DateTime GetMaxTimeStamp(string meteringName)
        {
            return DateTime.MinValue;
        }

        public void SaveCurrentMeterings(List<Metering> meterings)
        {

        }

        public void SaveJournalRows(string journalName, List<JournalDataRow> rows)
        {

        }

        public MySQLDataProvider()
        {

        }
    }
}
