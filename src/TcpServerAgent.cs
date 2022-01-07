using System;
using System.Net.Sockets;

namespace Zeloot.Tcp
{
    public class TcpServerAgent
    {
        public Socket socket { get; private set; }
        public bool init { get; private set; }
        public bool closed { get; private set; }

        public event TcpEvent.Open OnOpen;
        public event TcpEvent.Receive OnReceive;
        public event TcpEvent.Close OnClose;
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

        public TcpServerAgent(Socket socket)
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