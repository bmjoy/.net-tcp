using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

namespace Zeloot.Tcp
{
    public class TcpServer
    {
        public IPEndPoint host { get; private set; }
        public Socket socket { get; private set; }
        public bool IsListen { get; private set; }
        public List<TcpServerAgent> agents { get; private set; }
        private event TcpEvent.Open OnOpenEvent;
        private event TcpEvent.Close OnCloseEvent;
        private event TcpEvent.Receive OnReceiveEvent;


        private TcpServer(IPEndPoint host, Socket socket)
        {
            if (socket == null) socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            this.socket = socket;
            this.host = host;
            this.agents = new List<TcpServerAgent>();
            this.IsListen = false;
        }

        public static TcpServer Init(IPEndPoint host, Socket socket = null)
        {
            var _host = host;
            return new TcpServer(_host, socket);
        }

        public static TcpServer Init(IPAddress ip, int port, Socket socket = null)
        {
            var _host = new IPEndPoint(ip, port);
            return new TcpServer(_host, socket);
        }

        public static TcpServer Init(string ip, int port, Socket socket = null)
        {
            var _host = new IPEndPoint(IPAddress.Parse(ip), port);
            return new TcpServer(_host, socket);
        }

        public static TcpServer Init(TcpServer back, Socket socket = null)
        {
            var _host = new IPEndPoint(back.host.Address, back.host.Port);
            return new TcpServer(_host, socket);
        }

        public bool Open(int backlog = 1)
        {
            try
            {
                socket.Bind(host);
                socket.Listen(backlog);
                socket.BeginAccept(Accept, null);
#if UNITY_STANDALONE || UNITY_IOS || UNITY_WII || UNITY_ANDROID || UNITY_PS4 || UNITY_XBOXONE || UNITY_LUMIN || UNITY_TIZEN || UNITY_TVOS || UNITY_WEBGL || UNITY_ANALYTICS || UNITY_WINRT
                Zeloot.Tcp.MainThread.New();
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
            var agent = new TcpServerAgent(socket.EndAccept(result));
            agent.OnOpen += OnOpenEvent;
            agent.OnClose += OnCloseEvent;
            agent.OnReceive += OnReceiveEvent;
            agent.Init();
            agents.Add(agent);
            socket.BeginAccept(Accept, null);
        }

        public void OnOpen(Action<TcpServerAgent> action)
        {
            OnOpenEvent += (agent) =>
            {
#if UNITY_STANDALONE || UNITY_IOS || UNITY_WII || UNITY_ANDROID || UNITY_PS4 || UNITY_XBOXONE || UNITY_LUMIN || UNITY_TIZEN || UNITY_TVOS || UNITY_WEBGL || UNITY_ANALYTICS || UNITY_WINRT
                Zeloot.Tcp.MainThread.Instance?.Add(() =>
                {
                    action?.Invoke(agent);
                });
#else
                action?.Invoke(agent);
#endif
            };
        }

        public void OnClose(Action<TcpServerAgent> action)
        {
            OnCloseEvent += (agent) =>
            {
#if UNITY_STANDALONE || UNITY_IOS || UNITY_WII || UNITY_ANDROID || UNITY_PS4 || UNITY_XBOXONE || UNITY_LUMIN || UNITY_TIZEN || UNITY_TVOS || UNITY_WEBGL || UNITY_ANALYTICS || UNITY_WINRT
                Zeloot.Tcp.MainThread.Instance?.Add(() =>
                {
                    action?.Invoke(agent);
                });
#else
                action?.Invoke(agent);
#endif
            };
        }

        public void OnReceive(Action<TcpServerAgent, byte[]> action)
        {
            OnReceiveEvent += (agent, data) =>
            {
#if UNITY_STANDALONE || UNITY_IOS || UNITY_WII || UNITY_ANDROID || UNITY_PS4 || UNITY_XBOXONE || UNITY_LUMIN || UNITY_TIZEN || UNITY_TVOS || UNITY_WEBGL || UNITY_ANALYTICS || UNITY_WINRT
                Zeloot.Tcp.MainThread.Instance?.Add(() =>
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