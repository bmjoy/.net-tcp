using System;
using System.Net.Sockets;

namespace Zeloot.Tcp
{
    public class NetTcpServerAgent
    {
        public Socket socket { get; private set; }
        public bool init { get; private set; }
        public bool closed { get; private set; }

        public event NetTcpEvent.Open OnOpen;
        public event NetTcpEvent.Receive OnReceive;
        public event NetTcpEvent.Close OnClose;
        private const int bufferSize = 1024 * 4;
        private byte[] buffer;

        public bool IsConnected
        {
            get
            {
                try { return !(socket.Poll(5000, SelectMode.SelectRead) && socket.Available == 0); }
                catch { return false; }
            }
        }

        public NetTcpServerAgent(Socket socket)
        {
            this.socket = socket;
            this.buffer = new byte[bufferSize];
            this.init = false;
            this.closed = false;
        }

        public void Init()
        {
            if (init) return;
            init = true;

            OnOpen?.Invoke(this);

            BeginReceive();
        }
        public void Send(string data)
        {
            Send(NetTcpMain.Encode(data));
        }

        public void SendAsync(string data)
        {
            SendAsync(NetTcpMain.Encode(data));
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

        private void BeginReceive()
        {
            try
            {
                socket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, Receive, null);
            }
            catch
            {
                ErrorOnReceive();
            }
        }

        private void Receive(IAsyncResult result)
        {
            try
            {
                int size = socket.EndReceive(result);

                if (size > 0)
                {
                    byte[] data = new byte[size];
                    Array.Copy(buffer, data, size);
                    OnReceive?.Invoke(this, data);
                }
                else
                {
                    ErrorOnReceive();
                }
            }
            catch
            {
                ErrorOnReceive();
            }

            BeginReceive();
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
    }
}