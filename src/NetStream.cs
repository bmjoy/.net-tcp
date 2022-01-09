using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

namespace Zeloot.Tcp
{
    public class NetStream
    {
        private delegate void OnEvents(string name);
        private delegate void OnIOEvents(NetStream stream);
        private event OnEvents OnEvent;
        private event OnEvents OnIOEvent;
        private NetStreamIO io;
        private List<NetStreamIO> streamIOs;

        public readonly NetTcpServer server;

        public NetStream(string address, int port, Socket socket = null)
        {
            server = NetTcpServer.Init(address, port);
        }

        public NetStream(IPAddress address, int port, Socket socket = null)
        {
            server = NetTcpServer.Init(address, port, socket);
        }

        public NetStream(IPEndPoint host, Socket socket = null)
        {
            server = NetTcpServer.Init(host, socket);
        }

        public void Open(int backlog = 10)
        {
            server.Open(backlog);
            if (server.IsListen)
            {
                NetMainThread.Instance?.Add(() =>
                {
                    OnEvent?.Invoke("open");
                });

                streamIOs = new List<NetStreamIO>();
            }
            else
            {
                NetMainThread.Instance?.Add(() =>
                {
                    OnEvent?.Invoke("close");
                });
            }
        }

        public void On(string name, Action action)
        {
            OnEvent += (_name) =>
            {
                if (name == _name)
                {
                    NetMainThread.Instance?.Add(() =>
                    {
                        action?.Invoke();
                    });
                }
            };
        }

        public void On(string name, Action<NetStreamIO> action)
        {
            if (name == "connection")
            {
                server.OnOpen((agent) =>
                {
                    io = new NetStreamIO(agent);
                    streamIOs.Add(io);

                    NetMainThread.Instance?.Add(() =>
                    {
                        action?.Invoke(io);
                    });
                });
            }
        }

        public void Broadcast(string name, string data, bool async = true)
        {
            Broadcast(name, NetTcpMain.Encode(data), async);
        }

        public void Broadcast(string name, byte[] data, bool async = true)
        {
            foreach (var io in streamIOs)
            {
                try
                {
                    if (io.IsConnected) io.Emit(name, data, async);
                }
                catch { }
            }
        }

        public void Close()
        {
            server?.Close();
            streamIOs.Clear();

            NetMainThread.Instance?.Add(() =>
            {
                OnEvent?.Invoke("close");
            });
        }
    }
}
