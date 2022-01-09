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
#if UNITY_STANDALONE || UNITY_IOS || UNITY_WII || UNITY_ANDROID || UNITY_PS4 || UNITY_XBOXONE || UNITY_LUMIN || UNITY_TIZEN || UNITY_TVOS || UNITY_WEBGL || UNITY_ANALYTICS || UNITY_WINRT

                NetMainThread.Instance?.Add(() =>
                {
                    OnEvent?.Invoke("open");
                });
#else
                    OnEvent?.Invoke("open");
#endif

                streamIOs = new List<NetStreamIO>();
            }
            else
            {
#if UNITY_STANDALONE || UNITY_IOS || UNITY_WII || UNITY_ANDROID || UNITY_PS4 || UNITY_XBOXONE || UNITY_LUMIN || UNITY_TIZEN || UNITY_TVOS || UNITY_WEBGL || UNITY_ANALYTICS || UNITY_WINRT

                NetMainThread.Instance?.Add(() =>
                {
                    OnEvent?.Invoke("close");
                });
#else
                    OnEvent?.Invoke("close");
#endif
            }
        }

        public void On(string name, Action action)
        {
            OnEvent += (_name) =>
            {
                if (name == _name)
                {
#if UNITY_STANDALONE || UNITY_IOS || UNITY_WII || UNITY_ANDROID || UNITY_PS4 || UNITY_XBOXONE || UNITY_LUMIN || UNITY_TIZEN || UNITY_TVOS || UNITY_WEBGL || UNITY_ANALYTICS || UNITY_WINRT

                    NetMainThread.Instance?.Add(() =>
                    {
                        action?.Invoke();
                    });
#else
                        action?.Invoke();
#endif
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
#if UNITY_STANDALONE || UNITY_IOS || UNITY_WII || UNITY_ANDROID || UNITY_PS4 || UNITY_XBOXONE || UNITY_LUMIN || UNITY_TIZEN || UNITY_TVOS || UNITY_WEBGL || UNITY_ANALYTICS || UNITY_WINRT

                    NetMainThread.Instance?.Add(() =>
                    {
                        action?.Invoke(io);
                    });
#else
                        action?.Invoke(io);
#endif
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

#if UNITY_STANDALONE || UNITY_IOS || UNITY_WII || UNITY_ANDROID || UNITY_PS4 || UNITY_XBOXONE || UNITY_LUMIN || UNITY_TIZEN || UNITY_TVOS || UNITY_WEBGL || UNITY_ANALYTICS || UNITY_WINRT

            NetMainThread.Instance?.Add(() =>
            {
                OnEvent?.Invoke("close");
            });
#else
                OnEvent?.Invoke("close");
#endif
        }
    }
}
