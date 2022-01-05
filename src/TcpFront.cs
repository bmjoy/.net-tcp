using System;
using System.Net;
using System.Net.Sockets;

namespace Zeloot.Tcp
{
    public class TcpFront
    {
        public Socket socket { get; private set; }
        public IPEndPoint host { get; private set; }

        private delegate void MainEvent(TcpFront front);
        private delegate void MessageEvent(TcpFront front, byte[] data);

        private event MainEvent OnOpenEvent;
        private event MainEvent OnCloseEvent;
        private event MessageEvent OnMessageEvent;

        private TcpFront(ref IPEndPoint host, ref Socket socket)
        {
            if (socket == null) socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            this.socket = socket;
            this.host = host;
        }

        public static TcpFront Init(IPEndPoint host, Socket socket = null)
        {
            var _host = host;
            return new TcpFront(ref _host, ref socket);
        }

        public static TcpFront Init(IPAddress ip, int port, Socket socket = null)
        {
            var _host = new IPEndPoint(ip, port);
            return new TcpFront(ref _host, ref socket);
        }

        public static TcpFront Init(string ip, int port, Socket socket = null)
        {
            var _host = new IPEndPoint(IPAddress.Parse(ip), port);
            return new TcpFront(ref _host, ref socket);
        }

        public void Open(out bool error, int backlog = 0)
        {
            try
            {
                socket.Bind(host);
                socket.Listen(backlog);
                socket.BeginAccept(Accept, null);
                error = false;
            }
            catch
            {
                error = true;
            }
        }

        private void Accept(IAsyncResult result)
        {
            var client = socket.EndAccept(result);
        }
    }
}