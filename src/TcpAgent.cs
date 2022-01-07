using System;
using System.Net;
using System.Net.Sockets;

namespace Zeloot.Tcp
{
    public class TcpAgent
    {

        public Socket socket { get; private set; }
        private byte[] buffer;
        private bool closed;
        public event TcpEvent.Open OnOpen;
        public event TcpEvent.Close OnClose;
        public event TcpEvent.Receive OnReceive;
        public bool IsConnected => Connected(this);
        public static bool Connected(TcpAgent agent, SelectMode mode = SelectMode.SelectRead, int timeout = 5000)
        {
            if (agent == null || agent.socket == null || !agent.socket.Connected) return false;
            try { return !(agent.socket.Poll(timeout, mode) && agent.socket.Available == 0); }
            catch { return false; }
        }


        public TcpAgent(Socket socket)
        {
            this.socket = socket;
            closed = false;
        }

        internal void Init()
        {
            try
            {
                if (IsConnected)
                {
                    OnOpen?.Invoke(this);
                    BeginReceive();
                }
                else
                {
                    Close();
                }
            }
            catch { Close(); }
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
                OnClose?.Invoke(this);
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
                    OnReceive?.Invoke(this, data);
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