using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Globalization;

namespace AtlasExchangePlusClasses
{
    class RouterTaskGetTasksStatuses : RouterTask
    {
        private List<TaskStatus> tasksStatuses;

        public RouterTaskGetTasksStatuses(UInt32 routerId)
        {
            tag = "get_tasks_statuses";
            RouterId = routerId;
            tasksStatuses = new List<TaskStatus>();
        }

        protected override void loadParameters()
        {
            var data = Database.GetData("select date_format(max(task_metering_point.status_time), '%Y%m%d%H%i%s') " +
                "from task_metering_point join task on task_metering_point.task = task.id where task.router = " + RouterId);
            if (data != null && data.Length > 0 && data[0] != null && data[0].Length > 0)
            {
                attributes["min_time"] = data[0][0];
            }
        }

        protected override void parseResponseBody(XmlElement root)
        {
            var taskStatus = root.FirstChild;
            while (taskStatus != null)
            {
                if (taskStatus.Name == "status")
                {
                    var attrs = taskStatus.Attributes;
                    var meter = UInt32.Parse(attrs["meter"].Value);
                    var time = DateTime.ParseExact(attrs["time"].Value, "yyyyMMddHHmmss", CultureInfo.InvariantCulture); 
                    tasksStatuses.Add(new TaskStatus(UInt32.Parse(attrs["task"].Value), meter,  time, Byte.Parse(attrs["result"].Value), 
                        Database.GetMeteringPointId(meter, time.Date)));
                }
                taskStatus = taskStatus.NextSibling;
            }
            Database.ClearMeteringPointsCache();
        }

        protected override void saveResult()
        {
            var sql = "";
            var linkInfo = new Dictionary<UInt32, Dictionary<string, DateTime>>();
            foreach (var ts in tasksStatuses)
            {
                if (ts.MeteringPoint != 0)
                {
                    if (!linkInfo.ContainsKey(ts.MeteringPoint))
                    {
                        linkInfo[ts.MeteringPoint] = new Dictionary<string, DateTime>();
                    }
                    if (ts.Result == 0 || ts.Result == 2 || ts.Result == 3)
                    {
                        if (!linkInfo[ts.MeteringPoint].ContainsKey("success") || linkInfo[ts.MeteringPoint]["success"] < ts.Time)
                        {
                            linkInfo[ts.MeteringPoint]["success"] = ts.Time;
                        }
                    }
                    else
                    {
                        if (!linkInfo[ts.MeteringPoint].ContainsKey("try") || linkInfo[ts.MeteringPoint]["try"] < ts.Time)
                        {
                            linkInfo[ts.MeteringPoint]["try"] = ts.Time;
                        }
                    }
                    sql += (sql.Length > 0 ? "," : "") + String.Format("({0:d},{1:d},{2:yyyyMMddHHmmss},{3:d})", ts.Task, ts.MeteringPoint, ts.Time, ts.Result);
                    Database.ExecuteNonQuery("update task_metering_point set status = '" + ts.Result + "', status_time = '" + ts.Time.ToString("yyyyMMddHHmmss") +
                        "' where task = '" + ts.Task + "' and metering_point = '" + ts.MeteringPoint + "' AND (isnull(status) OR status <> 5)");
                }
                if (sql.Length > 1000 || (ts == tasksStatuses.Last() && sql.Length > 0))
                {
                    Database.ExecuteNonQuery("insert into task_log(task, metering_point, time, status) values " + sql + " on duplicate key update status = values(status)");
                    sql = "";
                }
            }
            updateLinkInfo(linkInfo);
            updateMeterPasswords();
            markCompletedMeterTasks();
        }

        private void updateLinkInfo(Dictionary<UInt32, Dictionary<string, DateTime>> linkInfo)
        {
            foreach (var mp in linkInfo.Keys)
            {
                if (linkInfo[mp].ContainsKey("success"))
                {
                    var success = linkInfo[mp]["success"].ToString("yyyyMMddHHmmss");
                    Log.Write("Success: " + success);
                    Database.ExecuteNonQuery("update metering_point set success = " + success + " where id = " + mp + " and (success < " + success + " or isnull(success))");
                }

                if (linkInfo[mp].ContainsKey("try"))
                {
                    var try_ = linkInfo[mp]["try"].ToString("yyyyMMddHHmmss");
                    Database.ExecuteNonQuery("update metering_point set try = " + try_ + " where id = " + mp + " and (try < " + try_ + " or isnull(try))");
                }
            }
        }

        private void updateMeterPasswords()
        {

        }

        private void markCompletedMeterTasks()
        {
            var completedTasks = new List<string>();
            var data = Database.GetData("select task.id " +
                "from task " +
                "left join task_metering_point on task.id = task_metering_point.task " +
                "and (task_metering_point.status in (1, 3, 4, 5) or isnull(task_metering_point.status)) " +
                "where task.router = " + RouterId + " and task.status = 3 and task.period = 0 " +
                "and isnull(task_metering_point.metering_point)");
            foreach (var ct in data)
            {
                completedTasks.Add(ct[0]);
            }
            if (completedTasks.Count > 0)
            {
                Database.ExecuteNonQuery("update task set status = 4, status_time = now() where id in ("
                    + String.Join(",", completedTasks.ToArray()) + ")");
            }
        }
    }
}
