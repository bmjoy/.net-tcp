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
        private event MessageEvent OnReceiveEvent;
        private bool closed;

        private TcpFront(ref IPEndPoint host, ref Socket socket)
        {
            if (socket == null) socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            this.socket = socket;
            this.host = host;
            this.closed = false;
            this.buffer = new byte[1024 * 8];
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

        public static TcpFront Init(TcpFront front, Socket socket = null)
        {
            var _host = new IPEndPoint(front.host.Address, front.host.Port);
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

        public static bool Connected(TcpFront front, SelectMode mode = SelectMode.SelectRead, int timeout = 5000)
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
            OnReceiveEvent += (data) =>
            {
                action?.Invoke(data);
            };
        }


        public void Open(out bool error, int timeout = 1000)
        {
            if (IsConnected) throw new Exception("server is running");

            error = true;

            try
            {
                try
                {
                    var result = socket.BeginConnect(host, Connect, null);
                    result.AsyncWaitHandle.WaitOne(timeout, true);
                    error = !IsConnected;
                }
                catch { }
            }
            catch { }
        }

        public void Send(string data)
        {
            Send(Encode(data));
        }

        public void SendAsync(string data)
        {
            SendAsync(Encode(data));
        }

        public void Send(byte[] data)
        {
            try
            {
                socket.Send(data, 0, data.Length, SocketFlags.None);
            }
            catch
            {
                if (IsConnected)
                {
                    BeginReceive();
                    SendAsync(data);
                }
                else Close();
            }
        }

        public void SendAsync(byte[] data)
        {
            try
            {
                socket.BeginSend(data, 0, data.Length, SocketFlags.None, null, null);
            }
            catch
            {
                if (IsConnected)
                {
                    BeginReceive();
                    SendAsync(data);
                }
                else Close();
            }
        }

        private void Connect(IAsyncResult result)
        {
            socket.EndConnect(result);

            if (!IsConnected)
            {
                OnCloseEvent?.Invoke();
                return;
            }

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
                if (IsConnected) BeginReceive();
                else Close();
            }

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
                else
                {
                    if (IsConnected) BeginReceive();
                    else Close();
                }
            }
            catch
            {
                if (IsConnected) BeginReceive();
                else Close();
            }
        }
    }
}