using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Zeloot.Tcp
{
    public class TcpFront
    {
        public Socket socket { get; private set; }
        public IPEndPoint host { get; private set; }

        private delegate void MainEvent();
        private delegate void MessageEvent(byte[] data);
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

        public static string Decode(byte[] data)
        {
            return Encoding.UTF8.GetString(data);
        }

        public static byte[] Encode(string data)
        {
            return Encoding.UTF8.GetBytes(data);
        }

        public void OnOpen(Action action)
        {
            OnOpenEvent += () =>
            {
                action?.Invoke();
            };
        }

        public void OnClose(Action action)
        {
            OnCloseEvent += () =>
            {
                action?.Invoke();
            };
        }

        public void OnReceive(Action<byte[]> action)
        {
            OnMessageEvent += (data) =>
            {
                action?.Invoke(data);
            };
        }


        public void Open(out bool error, int timeout = 1000)
        {
            error = true;

            try
            {
                try
                {
                    var result = socket.BeginConnect(host, Connect, null);
                    result.AsyncWaitHandle.WaitOne(timeout, true);
                    error = !socket.Connected;
                }
                catch { }
            }
            catch { }
        }

        private void Connect(IAsyncResult result)
        {
            socket.EndConnect(result);

            if (!socket.Connected) OnCloseEvent?.Invoke();

            OnOpenEvent?.Invoke();
            BeginReceive();

        }

        private void BeginReceive()
        {
            try
            {
                socket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, Receive, null);
            }
            catch
            {

            }
        }

        private void Receive(IAsyncResult result)
        {

        }
    }
}