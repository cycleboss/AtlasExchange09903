using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;

namespace AtlasExchangePlusClasses
{
    class RouterConnectionTcp: IRouterConnection
    {
        public IPAddress IpAddress { get; set; }
        private int port;
        private Socket socket;

        public bool IsOpen { get { return socket.Connected; } }
        public bool CanOpen { get { return IpAddress != null; } }

        public RouterConnectionTcp(IPAddress ipAddress, int port)
        {
            this.IpAddress = ipAddress;
            this.port = port;
            socket = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp) { ReceiveTimeout = 360000 };
        }

        public RouterConnectionTcp(Socket socket)
        {
            IpAddress = null;
            port = 0;
            this.socket = socket;
        }

        public bool Open()
        {
            if (!IsOpen && IpAddress != null)
            {
                socket.Connect(new IPEndPoint(IpAddress, port));
            }
            return IsOpen;
        }

        public bool Close()
        {
            try
            {
                if (IsOpen)
                {
                    socket.Disconnect(true);
                }
                return !IsOpen;
            }
            catch (SocketException)
            {
                return !IsOpen;
            }
        }

        public void Send(string msg)
        {
            var bytes = Encoding.UTF8.GetBytes(msg);
            var sentBytes = 0;
            while (sentBytes < bytes.Length)
            {
                var tmp = 0;
                try
                {
                    tmp = socket.Send(bytes, sentBytes, bytes.Length - sentBytes, SocketFlags.None);
                }
                catch (SocketException ex)
                {
                    Close();
                    throw new AtlasExchangeException(AtlasExchangeExceptionType.ConnectionFail, "SocketException: " + ex.SocketErrorCode);
                }
                if (tmp > 0)
                {
                    sentBytes += tmp;
                }
                else 
                {
                    Close();
                    throw new AtlasExchangeException(AtlasExchangeExceptionType.ConnectionFail, "Can't send message");
                }
            }
        }

        public string Receive()
        {
            var buffer = new byte[4096];
            var recvBytes = 0;
            try
            {
                recvBytes = socket.Receive(buffer, 4096, SocketFlags.None);
            }
            catch (SocketException ex)
            {
                if (ex.SocketErrorCode == SocketError.TimedOut)
                {
                    return null;
                }
                throw new AtlasExchangeException(AtlasExchangeExceptionType.ConnectionFail, "SocketException: " + ex.SocketErrorCode);
            }
            if (recvBytes == 0)
            {
                Close();
                throw new AtlasExchangeException(AtlasExchangeExceptionType.ConnectionFail, "Socket closed by server");
            }
            return Encoding.UTF8.GetString(buffer, 0, recvBytes);
        }
    }
}
