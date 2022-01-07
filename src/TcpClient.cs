using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Zeloot.Tcp
{
    public class TcpClient
    {
        public Socket socket { get; private set; }
        public IPEndPoint host { get; private set; }

        private delegate void MainEvent();
        private delegate void MessageEvent(byte[] data);
        private byte[] buffer;
        private event MainEvent OnOpenEvent;
        private event MainEvent OnCloseEvent;
        private event MessageEvent OnReceiveEvent;
        private bool closed;

        private TcpClient(IPEndPoint host, Socket socket)
        {
            if (socket == null) socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            this.socket = socket;
            this.host = host;
            this.closed = false;
            this.buffer = new byte[1024 * 8];
        }

        public static TcpClient Init(IPEndPoint host, Socket socket = null)
        {
            var _host = host;
            return new TcpClient(_host, socket);
        }

        public static TcpClient Init(IPAddress ip, int port, Socket socket = null)
        {
            var _host = new IPEndPoint(ip, port);
            return new TcpClient(_host, socket);
        }

        public static TcpClient Init(string ip, int port, Socket socket = null)
        {
            var _host = new IPEndPoint(IPAddress.Parse(ip), port);
            return new TcpClient(_host, socket);
        }

        public static TcpClient Init(TcpClient front, Socket socket = null)
        {
            var _host = new IPEndPoint(front.host.Address, front.host.Port);
            return new TcpClient(_host, socket);
        }

        public static bool Connected(TcpClient front, SelectMode mode = SelectMode.SelectRead, int timeout = 5000)
        {
            if (front == null || front.socket == null || !front.socket.Connected) return false;
            try { return !(front.socket.Poll(timeout, mode) && front.socket.Available == 0); }
            catch { return false; }
        }

        public bool IsConnected => Connected(this);

        public void OnOpen(Action action)
        {
            OnOpenEvent += () =>
            {
                Zeloot.Tcp.MainThread.Instance?.Add(() =>
                {
                    action?.Invoke();
                });
            };
        }

        public void OnClose(Action action)
        {
            OnCloseEvent += () =>
            {
                Zeloot.Tcp.MainThread.Instance?.Add(() =>
                {
                    action?.Invoke();
                });
            };
        }

        public void OnReceive(Action<byte[]> action)
        {
            OnReceiveEvent += (data) =>
            {
                Zeloot.Tcp.MainThread.Instance?.Add(() =>
                {
                    action?.Invoke(data);
                });
            };
        }


        public bool Open()
        {
            if (IsConnected) throw new Exception("client is running");

            try
            {
                socket.Connect(host);
                if (!socket.Connected) return false;
                OnOpenEvent?.Invoke();
                BeginReceive();
                Zeloot.Tcp.MainThread.New();
                return true;

            }
            catch { return false; }

        }

        public void Send(string data)
        {
            Send(TcpMain.Encode(data));
        }

        public void SendAsync(string data)
        {
            SendAsync(TcpMain.Encode(data));
        }

        public void Send(byte[] data)
        {
            try
            {
                socket.Send(data, 0, data.Length, SocketFlags.None);
            }
            catch { ErrorOnSend(data); }
        }

        public void SendAsync(byte[] data)
        {
            try
            {
                socket.BeginSend(data, 0, data.Length, SocketFlags.None, null, null);
            }
            catch { ErrorOnSend(data); }
        }

        private void BeginReceive()
        {
            try
            {
                socket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, Receive, null);
            }
            catch { ErrorOnReceive(); }
        }

        public void Close()
        {
            if (IsConnected || !closed)
            {
                socket?.Close();
                OnCloseEvent?.Invoke();
                closed = true;
            }
        }

        private void ErrorOnSend(byte[] data)
        {
            if (IsConnected)
            {
                BeginReceive();
                SendAsync(data);
            }
            else Close();
        }
        private void ErrorOnReceive()
        {
            if (IsConnected) BeginReceive();
            else Close();
        }

        private void Receive(IAsyncResult result)
        {
            try
            {
                var size = socket.EndReceive(result);

                if (size > 0)
                {
                    var data = new byte[size];
                    Array.Copy(buffer, data, size);
                    OnReceiveEvent?.Invoke(data);
                    BeginReceive();
                }
                else { ErrorOnReceive(); }
            }
            catch { ErrorOnReceive(); }
        }
    }
}