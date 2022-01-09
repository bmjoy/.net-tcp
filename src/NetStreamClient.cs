using System;
using System.Net;
using System.Net.Sockets;

namespace Zeloot.Tcp
{
    public class NetStreamClient
    {
        public readonly NetTcpClient client;
        internal bool IsConnected
        {
            get
            {
                if (client == null || client.socket == null) return false;
                return client.IsConnected;
            }
        }

        public NetStreamClient(string address, int port, Socket socket = null)
        {
            client = NetTcpClient.Init(address, port);
        }

        public NetStreamClient(IPAddress address, int port, Socket socket = null)
        {
            client = NetTcpClient.Init(address, port, socket);
        }

        public NetStreamClient(IPEndPoint host, Socket socket = null)
        {
            client = NetTcpClient.Init(host, socket);
        }

        public void Open()
        {
            client.Open();
        }

        public void Close()
        {
            client?.Close();
        }

        public void On(string name, Action action)
        {
            if (name == null) return;

            if (name == "open")
            {
                client.OnOpen(() =>
                {
                    NetMainThread.Instance?.Add(() =>
                    {
                        action?.Invoke();
                    });
                });
            }
            else if (name == "close")
            {
                client.OnClose(() =>
                {
                    NetMainThread.Instance?.Add(() =>
                    {
                        action?.Invoke();
                    });
                });
            }
        }

        public void On(string name, Action<byte[]> action)
        {
            if (name == null) return;

            client.OnReceive((data) =>
            {
                var buffer = NetTcpMain.NetStream.Decode(name, data);
                if (buffer == null) return;

                NetMainThread.Instance?.Add(() =>
                {
                    action?.Invoke(buffer);
                });
            });
        }

        public void Emit(string name, byte[] data, bool async = true)
        {
            if (async)
            {
                client.SendAsync(NetTcpMain.NetStream.Encode(name, data));
            }
            else
            {
                client.Send(NetTcpMain.NetStream.Encode(name, data));
            }
        }

        public void Emit(string name, string data, bool async = true)
        {
            if (async)
            {
                client.SendAsync(NetTcpMain.NetStream.Encode(name, data));
            }
            else
            {
                client.Send(NetTcpMain.NetStream.Encode(name, data));
            }
        }
    }
}