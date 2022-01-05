using System;
using System.Collections.Generic;
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
        private byte[] buffer;
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
                socket.BeginConnect(host, Connect, null);
                error = false;
            }
            catch
            {
                error = true;
            }
        }

        private void Connect(IAsyncResult result)
        {
            socket.EndConnect(result);
            OnOpenEvent?.Invoke(this);
            socket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, Receive, null);
        }

        private void Receive(IAsyncResult result)
        {

        }
    }
}