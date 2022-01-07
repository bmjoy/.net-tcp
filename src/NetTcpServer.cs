using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

namespace Zeloot.Tcp
{
    public class NetTcpServer
    {
        public IPEndPoint host { get; private set; }
        public Socket socket { get; private set; }
        public bool IsListen { get; private set; }
        public List<NetTcpServerAgent> agents { get; private set; }
        private event NetTcpEvent.Open OnOpenEvent;
        private event NetTcpEvent.Close OnCloseEvent;
        private event NetTcpEvent.Receive OnReceiveEvent;


        private NetTcpServer(IPEndPoint host, Socket socket)
        {
            if (socket == null) socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            this.socket = socket;
            this.host = host;
            this.agents = new List<NetTcpServerAgent>();
            this.IsListen = false;
        }

        public static NetTcpServer Init(IPEndPoint host, Socket socket = null)
        {
            var _host = host;
            return new NetTcpServer(_host, socket);
        }

        public static NetTcpServer Init(IPAddress ip, int port, Socket socket = null)
        {
            var _host = new IPEndPoint(ip, port);
            return new NetTcpServer(_host, socket);
        }

        public static NetTcpServer Init(string ip, int port, Socket socket = null)
        {
            var _host = new IPEndPoint(IPAddress.Parse(ip), port);
            return new NetTcpServer(_host, socket);
        }

        public static NetTcpServer Init(NetTcpServer back, Socket socket = null)
        {
            var _host = new IPEndPoint(back.host.Address, back.host.Port);
            return new NetTcpServer(_host, socket);
        }

        public bool Open(int backlog = 1)
        {
            try
            {
                socket.Bind(host);
                socket.Listen(backlog);
                socket.BeginAccept(Accept, null);
#if UNITY_STANDALONE || UNITY_IOS || UNITY_WII || UNITY_ANDROID || UNITY_PS4 || UNITY_XBOXONE || UNITY_LUMIN || UNITY_TIZEN || UNITY_TVOS || UNITY_WEBGL || UNITY_ANALYTICS || UNITY_WINRT
                NetMainThread.New();
#endif
                IsListen = true;
            }
            catch
            {
                IsListen = false;
            }

            return IsListen;
        }

        public void Close()
        {
            if (!IsListen) return;
            IsListen = false;

            if (agents != null && agents.Count > 0)
            {
                foreach (var agent in agents)
                {
                    agent?.Close();
                }

                agents.Clear();
            }
        }

        private void Accept(IAsyncResult result)
        {
            var agent = new NetTcpServerAgent(socket.EndAccept(result));
            agent.OnOpen += OnOpenEvent;
            agent.OnClose += OnCloseEvent;
            agent.OnReceive += OnReceiveEvent;
            agent.Init();
            agents.Add(agent);
            socket.BeginAccept(Accept, null);
        }

        public void OnOpen(Action<NetTcpServerAgent> action)
        {
            OnOpenEvent += (agent) =>
            {
#if UNITY_STANDALONE || UNITY_IOS || UNITY_WII || UNITY_ANDROID || UNITY_PS4 || UNITY_XBOXONE || UNITY_LUMIN || UNITY_TIZEN || UNITY_TVOS || UNITY_WEBGL || UNITY_ANALYTICS || UNITY_WINRT
                NetMainThread.Instance?.Add(() =>
                {
                    action?.Invoke(agent);
                });
#else
                action?.Invoke(agent);
#endif
            };
        }

        public void OnClose(Action<NetTcpServerAgent> action)
        {
            OnCloseEvent += (agent) =>
            {
#if UNITY_STANDALONE || UNITY_IOS || UNITY_WII || UNITY_ANDROID || UNITY_PS4 || UNITY_XBOXONE || UNITY_LUMIN || UNITY_TIZEN || UNITY_TVOS || UNITY_WEBGL || UNITY_ANALYTICS || UNITY_WINRT
                NetMainThread.Instance?.Add(() =>
                {
                    action?.Invoke(agent);
                });
#else
                action?.Invoke(agent);
#endif
            };
        }

        public void OnReceive(Action<NetTcpServerAgent, byte[]> action)
        {
            OnReceiveEvent += (agent, data) =>
            {
#if UNITY_STANDALONE || UNITY_IOS || UNITY_WII || UNITY_ANDROID || UNITY_PS4 || UNITY_XBOXONE || UNITY_LUMIN || UNITY_TIZEN || UNITY_TVOS || UNITY_WEBGL || UNITY_ANALYTICS || UNITY_WINRT
                NetMainThread.Instance?.Add(() =>
                {
                    action?.Invoke(agent, data);
                });
#else
                action?.Invoke(agent, data);
#endif
            };
        }
    }
}