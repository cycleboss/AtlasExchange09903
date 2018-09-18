using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Data.Common;
using System.Globalization;
using MySql.Data.MySqlClient;

namespace AtlasExchangePlusClasses
{
    public class AtlasExchangePlus
    {   
        private volatile Dictionary<UInt32, Router> connectedRouters;
        private volatile Dictionary<UInt32, RouterTask> routerTasks;
        private volatile TcpListener server;

        Thread serverThread;
        Thread clientThread;

        private volatile bool run;
        private volatile bool gsmBusy;

        public AtlasExchangePlus(string dbType, string connectionString, int port = 5679)
        {
            Database.Init(dbType, connectionString);
            connectedRouters = new Dictionary<UInt32, Router>();
            routerTasks = new Dictionary<UInt32, RouterTask>();
            server = new TcpListener(IPAddress.Any, port);

            run = false;
            gsmBusy = true; //!! temporarily unsupported 09902
        }

        public void Start()
        {
            serverThread = new Thread(new ThreadStart(startServer));
            clientThread = new Thread(new ThreadStart(startClient));

            run = true;
            serverThread.Start();
            clientThread.Start();

        }

        public void Stop()
        {
            run = false;
            serverThread.Join();
            clientThread.Join();
        }

        private void startServer()
        {
            server.Start();
            Log.Write("Server started");
            while (run)
            {
                if (server.Pending())
                {
                    var socket = server.AcceptSocket();
                    Log.Write("Client connect from " + ((IPEndPoint)(socket.RemoteEndPoint)).Address.ToString());
                    new Thread(new ParameterizedThreadStart(onAcceptSocket)).Start(socket);
                }
                else
                {
                    delay(100);
                }
            }
            server.Stop();
            Log.Write("Server stopped");
        }

        private void startClient()
        {
            Log.Write("Client started");
            while (run)
            {
                //try
                {
                    var tasks = loadTasks();
                    foreach (var r in tasks.Keys)
                    {
                        routerTasks[r] = tasks[r];
                        if (connectedRouters.ContainsKey(r))
                        {
                            new Thread(new ParameterizedThreadStart(startRouterTask)).Start(connectedRouters[r]);
                        }
                        else
                        {
                            var router = loadRouter(r);
                            new Thread(new ParameterizedThreadStart(startConnectingRouter)).Start(router);
                        }
                    }
                }
                /*catch (Exception ex)
                {
                }*/
                delay(10000);
            }
            Log.Write("Client stopped");
        }

        private Dictionary<UInt32, RouterTask> loadTasks()
        {
            var tasks = new Dictionary<UInt32, RouterTask> ();
            try
            {
                var now = DateTime.Now.ToString("yyyyMMddHHmmss");

                var tasksArray = Database.GetData("select router_task_queue.router, router_task_queue.task, date_format(router_task_queue.time, '%Y%m%d%H%i%s'), " +
                    "router_task.period, date_format(router_task.start_date, '%Y%m%d') from router_task_queue join router on router_task_queue.router = router.id " +
                    "left join router_task on router_task_queue.router = router_task.router and router_task_queue.task = router_task.type " +
                    "where router_task_queue.time <= " + now + " and router.channel = 'tcp' and router.id not in (" +
                    (routerTasks.Count == 0 ? "0" : String.Join(",", routerTasks.Keys)) + ") and router.type = 9903" +
                    " order by router_task_queue.priority desc");
                int count = 0;
                foreach (var task in tasksArray)
                {
                    var routerId = UInt32.Parse(task[0]);
                    if (tasks.ContainsKey(routerId))
                    {
                        continue; // only one task for each router
                    }
                    UInt32 period = task[3] == null ? 0 : UInt32.Parse(task[3]);
                    DateTime startTime = DateTime.ParseExact(task[2], "yyyyMMddHHmmss", CultureInfo.InvariantCulture);
                    tasks[routerId] = RouterTask.Create((RouterTaskType)byte.Parse(task[1]), routerId, startTime, period);
                    ++count;
                }
                Log.Write(count + " tasks loaded");
                return tasks;
            }
            catch (MySqlException ex)
            {
                Log.Write("MySQL exception during load tasks: " + ex.Message);
                Database.FreeCommand();
                return tasks;
            }
            catch (Exception ex)
            {
                Log.Write("Exception during load tasks: " + ex.Message);
                return tasks;
            }
        }

        private RouterTask loadTask(UInt32 routerId)
        {
            try
            {
                var taskStr = Database.GetData("select router_task_queue.router, router_task_queue.task, date_format(router_task_queue.time, '%Y%m%d%H%i%s'), " +
                    "router_task.period, date_format(router_task.start_date, '%Y%m%d') from router_task_queue " +
                    "left join router_task on router_task_queue.router = router_task.router and router_task_queue.task = router_task.type " +
                    "where router_task_queue.time <= " + DateTime.Now.ToString("yyyyMMddHHmmss") + " and router_task_queue.router = " + routerId +
                    " order by router_task_queue.priority desc, router_task_queue.time");
                if (taskStr == null || taskStr.Length == 0)
                {
                    return null;
                }
                UInt32 period = taskStr[0][3] == null ? 0 : UInt32.Parse(taskStr[0][3]);
                DateTime startTime = DateTime.ParseExact(taskStr[0][2], "yyyyMMddHHmmss", CultureInfo.InvariantCulture);
                return RouterTask.Create((RouterTaskType)byte.Parse(taskStr[0][1]), routerId, startTime, period);
            }
            catch (Exception ex)
            {
                Log.Write("Exception during load task: " + ex.Message);
                return null;
            }
        }

        private Router loadRouter(string number, UInt32 type)
        {
            return loadRouter(0, number, type);
        }

        private Router loadRouter(UInt32 id)
        {
            return loadRouter(id, null, 0);
        }

        private Router loadRouter(UInt32 routerId, string routerNumber, UInt32 routerType)
        {
            var condition = routerId > 0 ? "id = " + routerId : "number = " + routerNumber + " and type = " + routerType;
            var reader = Database.GetReader("select id, type, number, channel, ipaddress, port, password, ipaddress2 from router where " + condition + " limit 1");
            Router ret = null;
            if (reader.Read())
            {
                var id = UInt32.Parse(reader.GetString(0));
                var type = UInt32.Parse(reader.GetString(1));
                var number = reader.GetString(2);
                var channel = reader.GetString(3);
                string address = null;
                try
                {
                    address = reader.GetString(4);
                }
                catch
                {
                }
                string address2 = null;
                try
                {
                    address2 = reader.GetString(7);
                }
                catch 
                {
                }
                if (address == null && address2 == null)
                {
                    return ret;
                }
                string password = reader.GetString(6);
                switch (channel)
                {
                    case "tcp":
                        {
                            IPAddress ipAddress = null;
                            try
                            {
                                ipAddress = IPAddress.Parse(address);
                            }
                            catch
                            {
                            }
                            IPAddress ipAddress2 = null;
                            try
                            {
                                ipAddress2 = IPAddress.Parse(address2);
                            }
                            catch 
                            {
                            }
                            if (ipAddress == null && ipAddress2 == null)
                            {
                                Database.FreeReader(reader);
                                return ret;
                            }
                            var port = 5679;
                            try
                            {
                                port = int.Parse(reader.GetString(5));
                            }
                            catch
                            {
                            }
                            if (type == 9903)
                            {
                                ret = new Router09903(routerId, number, password, ipAddress, port, ipAddress2);
                            }
                            else if (type == 9902)
                            {
                                //ret = new Router09902();
                            }
                            else
                            {
                            }
                            break;
                        }
                    case "gsm":
                        {
                            if (type == 9902)
                            {
                            }
                            else
                            {
                            }
                            break;
                        }
                    default:
                        {
                            break;
                        }
                }
            }
            Database.FreeReader(reader);
            return ret;
        }

        private Router getConnectedRouter(UInt32 routerId)
        {
            return connectedRouters.ContainsKey(routerId) ? connectedRouters[routerId] : null;
        }

        private void delay(int milliseconds)
        {
            int step = 100;
            while (milliseconds > 0 && run)
            {
                Thread.Sleep(step < milliseconds ? step : milliseconds);
                milliseconds -= step;
            }
        }

        private void onConnectRouter(Router router)
        {
            if (router != null && router.Login())
            {
                connectedRouters[router.Id] = router;
                startRouterTask(router);
            }
            else if (routerTasks.ContainsKey(router.Id))
            {
                routerTasks.Remove(router.Id);
            }
        }

        private void onAcceptSocket(object obj)
        {
            var routerNumber = Router09903.GetRouterNumber(obj as Socket);
            if (routerNumber != null)
            {
                Log.Write("Router " + routerNumber + " initiative connected");
                var router = loadRouter(routerNumber, 09903);
                onConnectRouter(router);
            }
            Database.FreeCommand();
        }

        private void startRouterTask(object obj)
        {
            var router = obj as Router;
            while (router != null && routerTasks.ContainsKey(router.Id) && run)
            {
                var task = routerTasks[router.Id];
                Log.Write("Start task " + task.Type + " for router " + router.Number + "...");
                try
                {
                    router.DoTask(task);
                    if (task.Status == RouterTaskStatus.Complete)
                    {
                        if (task.Result == RouterTaskResult.Success)
                        {
                            Log.Write("Task " + task.Type + " for router " + router.Number + " successfully completed.");
                        }
                        else
                        {
                            Log.Write("Task " + task.Type + " for router " + router.Number + " completed completed with error " + task.Result);
                        }
                    }
                    else
                    {
                        Log.Write("Task " + task.Type + " for router " + router.Number + " not completed.");
                    }

                    if (!router.IsConnected)
                    {
                        connectedRouters.Remove(router.Id);
                        Database.ExecuteNonQuery("update router set status = 2, status_time = " + DateTime.Now.ToString("yyyyMMddHHmmss") + " where id = " + router.Id);
                        Log.Write("Router " + router.Number + " disconnected");
                        break;
                    }
                }
                catch (Exception ex)
                {
                    Log.Write("Exception during run task " + task.Type + " for router " + router.Number + ": " + ex.Message + " " + ex.StackTrace);
                    break;
                }
                var nextTask = loadTask(router.Id);
                if (nextTask == null)
                {
                    routerTasks.Remove(router.Id);
                }
                else
                {
                    routerTasks[router.Id] = nextTask;
                }
            }
            if (routerTasks.ContainsKey(router.Id))
            {
                routerTasks.Remove(router.Id);
            }
            Database.FreeCommand();
        }

        private void startConnectingRouter(object obj)
        {
            var router = obj as Router;
            if (router == null)
            {
                Thread.Sleep(1000);
                Database.FreeCommand();
                return;
            }
            try
            {
                Log.Write("Connecting to router " + router.Number + "...");
                if (router.Connect())
                {
                    Database.ExecuteNonQuery("update router set status = 1, status_time = " + DateTime.Now.ToString("yyyyMMddHHmmss") + " where id = " + router.Id);
                    Log.Write("Router " + router.Number + " connected.");
                    onConnectRouter(router);
                }
                else
                {
                    Log.Write("Can't connect to router " + router.Number);
                    routerTasks.Remove(router.Id);
                }
            }
            catch
            {
                routerTasks.Remove(router.Id);
            }
            Database.FreeCommand();
        }
    }
}