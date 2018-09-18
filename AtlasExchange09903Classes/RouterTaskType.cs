using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AtlasExchangePlusClasses
{
    enum RouterTaskType
    {
        GetLoginInfo = 1,
        GetDeviceInfo = 2,
        Login = 3,
        SetMeters = 4,
        GetMeters = 5,
        SetTasks = 6,
        GetTasks = 7,
        SetTariffPlans = 8,
        GetTariffPlans = 9,
        SetConfigTemplates = 10,
        GetTariffTemplates = 11,
        GetTasksStatuses = 12,
        GetCurrentMeterings = 13,
        GetJournalDaily = 14,
        GetJournalLoadProfile = 15,
        GetJournalOnOff = 16,
        GetJournalEnergyQuality = 17,
        GetJournalVoltage = 18,
        GetJournalCurrent = 19,
        GetJournalExternalInfluence = 20,
        GetJournalSelftest = 21,
        GetJournalCorrections = 22,
        GetJournalTg = 23,
        GetJournalDiscrete = 24,
        GetJournalMonthly = 25,
        GetCoordinates = 26,
    }
}
