using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace AtlasExchangePlusClasses
{
    class RouterTask : IRouterTask09903, IRouterTask09902
    {
        protected string tag;
        protected Dictionary<string, string> attributes = new Dictionary<string, string>();
        protected UInt32 packageNumber = 0;

        public RouterTaskType Type { get; private set; }
        public DateTime Time { get; private set; }
        public UInt32 Period { get; private set; }

        public UInt32 RouterId { get; protected set; }

        public RouterTaskResult Result;
        public RouterTaskStatus Status;

        public bool IsPeriodical { get { return Period > 0; } }
        public DateTime NextStartTime 
        { 
            get 
            {
                if (!IsPeriodical)
                {
                    return DateTime.MaxValue;
                }
                var nextTime = Time.AddSeconds(Period);
                while (nextTime < DateTime.Now)
                {
                    nextTime = nextTime.AddSeconds(Period);
                }
                return nextTime;
            } 
        }

        public static RouterTask Create(RouterTaskType type, UInt32 routerId, DateTime startTime, UInt32 period)
        {
            RouterTask task;
            switch (type)
            {
                case RouterTaskType.GetLoginInfo:
                    {
                        task = new RouterTaskGetLoginInfo();
                        break;
                    }
                case RouterTaskType.GetDeviceInfo:
                    {
                        task = new RouterTaskGetDeviceInfo(routerId);
                        break;
                    }
                case RouterTaskType.Login:
                    {
                        task = new RouterTaskLogin(routerId);
                        break;
                    }
                case RouterTaskType.SetMeters:
                    {
                        task = new RouterTaskSetMeters(routerId);
                        break;
                    }
                case RouterTaskType.SetTasks:
                    {
                        task = new RouterTaskSetTasks(routerId);
                        break;
                    }
                case RouterTaskType.SetTariffPlans:
                    {
                        task = new RouterTaskSetTariffPlans(routerId);
                        break;
                    }
                case RouterTaskType.SetConfigTemplates:
                    {
                        task = new RouterTaskSetConfigurationTemplates(routerId);
                        break;
                    }
                case RouterTaskType.GetTasksStatuses:
                    {
                        task = new RouterTaskGetTasksStatuses(routerId);
                        break;
                    }
                case RouterTaskType.GetCurrentMeterings:
                    {
                        task = new RouterTaskGetCurrentMeterings(routerId);
                        break;
                    }
                case RouterTaskType.GetJournalDaily:
                    {
                        task = new RouterTaskGetJournalDaily(routerId);
                        break;
                    }
                case RouterTaskType.GetJournalLoadProfile:
                    {
                        task = new RouterTaskGetJournalLoadProfile(routerId);
                        break;
                    }
                case RouterTaskType.GetJournalOnOff:
                    {
                        task = new RouterTaskGetJournalOnOff(routerId);
                        break;
                    }
                case RouterTaskType.GetJournalEnergyQuality:
                    {
                        task = new RouterTaskGetJournalEnergyQuality(routerId);
                        break;
                    }
                case RouterTaskType.GetJournalVoltage:
                    {
                        task = new RouterTaskGetJournalVoltage(routerId);
                        break;
                    }
                case RouterTaskType.GetJournalCurrent:
                    {
                        task = new RouterTaskGetJournalCurrent(routerId);
                        break;
                    }
                case RouterTaskType.GetJournalExternalInfluence:
                    {
                        task = new RouterTaskGetJournalExternalInfluence(routerId);
                        break;
                    }
                case RouterTaskType.GetJournalSelftest:
                    {
                        task = new RouterTaskGetJournalSelftest(routerId);
                        break;
                    }
                case RouterTaskType.GetJournalCorrections:
                    {
                        task = new RouterTaskGetJournalCorrections(routerId);
                        break;
                    }
                case RouterTaskType.GetJournalTg:
                    {
                        task = new RouterTaskGetJournalTg(routerId);
                        break;
                    }
                case RouterTaskType.GetJournalDiscrete:
                    {
                        task = new RouterTaskGetJournalDiscrete(routerId);
                        break;
                    }
                case RouterTaskType.GetJournalMonthly:
                    {
                        task = new RouterTaskGetJournalMonthly(routerId);
                        break;
                    }
                case RouterTaskType.GetCoordinates:
                    {
                        task = new RouterTaskGetCoordinates(routerId);
                        break;
                    }
                default:
                    {
                        throw new ArgumentOutOfRangeException("Unknown router task type: " + type);
                    }
            }
            task.Type = type;
            task.Time = startTime;
            task.Period = period;
            task.loadParameters();
            task.loadDataMinDate();
            task.Status = RouterTaskStatus.Inited;
            task.Result = RouterTaskResult.Success;
            return task;
        }

        XmlDocument IRouterTask09903.CreateRequest()
        {
            if (Status == RouterTaskStatus.Inited)
            {
                Status = RouterTaskStatus.Started;
            }
            var request = new XmlDocument();
            var root = request.CreateElement(tag);
            foreach (var attr in attributes)
            {
                root.SetAttribute(attr.Key, attr.Value);
            }
            if (packageNumber > 0)
            {
                root.SetAttribute("package_number", packageNumber.ToString());
            }
            request.AppendChild(root);
            createRequestBody(request);
            return request;
        }

        XmlDocument IRouterTask09903.ParseResponse(XmlDocument response)
        {
            Result = checkValidResponse(response);
            if (Result != RouterTaskResult.Success)
            {
                handleError();
                return null;
            }
            var root = response.DocumentElement;
            parseResponseBody(root);

            if (root.Attributes["package_number"] == null || root.Attributes["last_package"] != null)
            {
                Status = RouterTaskStatus.Complete;
                return null;
            }
            packageNumber = UInt32.Parse(root.Attributes["package_number"].Value);
            var request = ((IRouterTask09903)this).CreateRequest();

            return request;
        }

        string[] IRouterTask09902.CreateRequest()
        {
            throw new NotImplementedException();
        }

        string[] IRouterTask09902.ParseResponse(string response)
        {
            throw new NotImplementedException();
        }

        public void Complete()
        {
            saveResult();
            if (Status == RouterTaskStatus.Complete && Result == RouterTaskResult.Success)
            {
                updateStartTime();
            }
        }

        protected virtual void loadParameters()
        {

        }

        protected virtual void saveResult()
        {

        }

        protected virtual void loadDataMinDate()
        {
            var data = Database.GetData("select date_format(start_date, '%Y%m%d') from router_task where router = " + RouterId + 
                " and type = " + (int)Type + " limit 1");
            if (data != null && data.Length > 0 && data[0] != null && data[0].Length > 0 && data[0][0] != null)
            {
                attributes["min_date"] = data[0][0];
            }
        }

        protected virtual void createRequestBody(XmlDocument request)
        {
        }

        protected virtual void parseResponseBody(XmlElement root)
        {
        }

        private void handleError()
        {
            Status = RouterTaskStatus.Complete;
            switch (Result)
            {
                case RouterTaskResult.AuthenticationFailed:
                    {
                        break;
                    }
                case RouterTaskResult.UnknownCommand:
                    {
                        break;
                    }
                case RouterTaskResult.RuntimeError:
                    {
                        break;
                    }
                default:
                    {
                        break;
                    }
            }
        }

        protected virtual RouterTaskResult checkValidResponse(XmlDocument response)
        {
            var root = response.DocumentElement;
            if (root.Name != tag)
            {
                throw new Exception("Invalid response, excpected XML node '" + tag + "', received '" + root.Name + "'");
            }
            var res = root.Attributes["res"];
            if (res == null)
            {
                throw new Exception("Attribute res not found in response '" + root.Name + "'");
            }
            RouterTaskResult ret;
            try
            {
                ret = (RouterTaskResult)(Byte.Parse(res.Value));
            }
            catch (FormatException)
            {
                throw new ArgumentOutOfRangeException("Attribute res in " + root.Name + " has invalid value '" + res.Value + "'");
            }
            return ret;
        }

        private void updateStartTime()
        {
            if (IsPeriodical)
            {
                Database.ExecuteNonQuery("update router_task set last_time = " + DateTime.Now.ToString("yyyyMMddHHmmss") +
                    " where router = " + RouterId + " and type = " + (int)Type);
                Database.ExecuteNonQuery("update router_task_queue set time = " + NextStartTime.ToString("yyyyMMddHHmmss") +
                    " where router = " + RouterId + " and task = " + (int)Type);
            }
            else
            {
                Database.ExecuteNonQuery("delete from router_task_queue where router = " + RouterId + " and task = " + (int)Type +
                    " and time = " + Time.ToString("yyyyMMddHHmmss"));
            }
        }
    }
}
