using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace AtlasExchangePlusClasses
{
    class RouterTaskSetTasks : RouterTask
    {
        private Dictionary<string, MeterTask> tasks;

        public RouterTaskSetTasks(UInt32 routerId)
        {
            tag = "set_tasks";
            RouterId = routerId;
        }

        protected override void loadParameters()
        {
            tasks = new Dictionary<string, MeterTask>();      
            var tasksArray = Database.GetData("select id, type, period_type, period, priority from task where router = " + RouterId + " and status in (2, 3)");
            if (tasksArray == null || tasksArray.Length == 0)
            {
                return;
            }
            foreach (var task in tasksArray)
            {
                var period = UInt32.Parse(task[3]);
                switch (task[2])
                {
                    case "1":
                        {
                            period *= 60;
                            break;
                        }
                    case "2":
                        {
                            period *= 60 * 60;
                            break;
                        }
                    case "3":
                        {
                            period *= 60 * 60 * 24;
                            break;
                        }
                    default:
                        {
                            break;
                        }
                }
                tasks[task[0]] = new MeterTask(task[0], task[1], task[2] == "0" ? "0" : "1", period.ToString(), task[4]);
            }
            
            var paramsArray = Database.GetData("select task, param, value from task_param where task in (" + String.Join(",", tasks.Keys) + ")");
            foreach (var param in paramsArray)
            {
                tasks[param[0]].AddParameter(param[1], param[2]);
            }

            var metersArray = Database.GetData("select task_metering_point.task, metering_point_meter.meter from metering_point_meter " +
                    "join task_metering_point on task_metering_point.metering_point = metering_point_meter.metering_point " +
                    "where task_metering_point.task in (" + String.Join(",", tasks.Keys) + ") and isnull(metering_point_meter.end_time)");
            foreach (var meter in metersArray)
            {
                tasks[meter[0]].AddMeter(meter[1]);
            }
        }

        protected override void createRequestBody(XmlDocument request)
        {
            var root = request.DocumentElement;
            foreach (var task in tasks.Keys)
            {
                tasks[task].ToXml(request, root);
            }
        }

        protected override void saveResult()
        {
            if (tasks.Count > 0)
            {
                Database.ExecuteNonQuery("update task set status = 3 where status = 2 and id in (" + String.Join(",", tasks.Keys) + ")");
            }
        }
    }
}
