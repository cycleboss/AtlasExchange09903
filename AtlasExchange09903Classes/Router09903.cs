using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Xml;
using System.Globalization;

namespace AtlasExchangePlusClasses
{
    class Router09903 : Router
    {
        private RouterConnectionTcp connection2;

        public string ProtocolVersion { get; private set; }
        public string Encryption { get; private set; }

        private Router09903(Socket socket)
        {
            Number = null;
            connection = new RouterConnectionTcp(socket);
        }

        public override bool Connect()
        {
            try
            {
                Log.Write("try connect to " + ((RouterConnectionTcp)connection).IpAddress.ToString());
                if (connection.IsOpen || connection.Open())
                {
                    return true;
                }
            }
            catch (SocketException ex)
            {
                Log.Write(ex.Message);
            }
            Log.Write("Can't connect to " + ((RouterConnectionTcp)connection).IpAddress.ToString());
            if (connection2 == null)
            {
                return false;
            }

            Log.Write("try connect to " + connection2.IpAddress.ToString());
            connection = connection2;
            try
            {
                return connection.IsOpen || connection.Open();
            }
            catch (SocketException)
            {
            }
            Log.Write("Can't connect to " + ((RouterConnectionTcp)connection).IpAddress.ToString());
            return false;
        }



        public Router09903(UInt32 id, string number, string password, IPAddress ipAddress, int port, IPAddress ipAddress2 = null)
        {
            init(id, RouterType.RiM09903, number, password);
            connection = new RouterConnectionTcp(ipAddress, port);
            if (ipAddress2 != null)
            {
                connection2 = ipAddress2 != null ? new RouterConnectionTcp(ipAddress2, port) : null;
            }
        }

        public static string GetRouterNumber(Socket socket)
        {
            var router = new Router09903(socket);
            var deviceInfoTask = (RouterTaskGetDeviceInfo)RouterTask.Create(RouterTaskType.GetDeviceInfo, 0, DateTime.Now, 0);
            router.DoTask(deviceInfoTask);
            return deviceInfoTask.RouterNumber;
        }

        public override void DoTask(RouterTask task)
        {
            var task09903 = task as IRouterTask09903;
            var request = task09903.CreateRequest();
            try
            {
                while (request != null)
                {
                    var response = exchange(request);
                    request = task09903.ParseResponse(response);
                }
            }
            catch (AtlasExchangeException ex)
            {
                Log.Write("AtlasExchangeException: " + ex.Message);
            }
            catch (Exception ex)
            {
                Log.Write("Unexpected exception: " + ex.StackTrace);
            }
            finally
            {
                task.Complete();
            }
        }

        public override bool Login()
        {
            var loginInfo = (RouterTaskGetLoginInfo)RouterTask.Create(RouterTaskType.GetLoginInfo, Id, DateTime.Now, 0);
            DoTask(loginInfo);
            if (loginInfo.Status != RouterTaskStatus.Complete || loginInfo.Result != RouterTaskResult.Success)
            {
                return false;
            }
            ProtocolVersion = loginInfo.Protocol;
            Encryption = loginInfo.Encryption;
            var login = RouterTask.Create(RouterTaskType.Login, Id, DateTime.Now, 0);
            DoTask(login);
            return login.Status == RouterTaskStatus.Complete && login.Result == RouterTaskResult.Success;
        }

        private XmlDocument exchange(XmlDocument request)
        {
            send(request);
            var response = receive();
            return response.DocumentElement.Name == request.DocumentElement.Name ? response : null;
        }

        private void send(XmlDocument doc)
        {
            //Log.Write("Send: " + doc.OuterXml);
            connection.Send(doc.OuterXml);
        }

        private XmlDocument receive()
        {
            var ret = new XmlDocument();
            var reply = "";
            while (true)
            {
                var part = connection.Receive();
                
                if (String.IsNullOrEmpty(part))
                {
                    if (reply.Length > 0)
                    {
                        try
                        {
                            ret.LoadXml(reply);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.StackTrace);
                        }
                        throw new AtlasExchangeException(AtlasExchangeExceptionType.WrongResponse, "Invalid response: " + reply);
                    }
                    else
                    {
                        throw new AtlasExchangeException(AtlasExchangeExceptionType.NoResponse, "No response");
                    }
                }
                reply += part;
                try
                {
                    ret.LoadXml(reply);
                    //Log.Write("Received: " + reply);
                    return ret;
                }
                catch (XmlException ex)
                {
                    Log.Write(ex.Data.ToString());
                }
                catch (Exception ex)
                {
                    throw new AtlasExchangeException(AtlasExchangeExceptionType.UnknownException, ex.StackTrace);
                }
            }
        }
    }
}
