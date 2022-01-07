using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Zeloot.Tcp
{
    public class TcpBack
    {
        public IPEndPoint host { get; private set; }
        public Socket socket { get; private set; }
        public bool IsListen { get; private set; }
        public List<TcpAgent> agents { get; private set; }
        private event TcpEvent.Open OnOpenEvent;
        private event TcpEvent.Close OnCloseEvent;
        private event TcpEvent.Receive OnReceiveEvent;


        private TcpBack(IPEndPoint host, Socket socket)
        {
            if (socket == null) socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            this.socket = socket;
            this.host = host;
            this.agents = new List<TcpAgent>();
            this.IsListen = false;
        }

        public static TcpBack Init(IPEndPoint host, Socket socket = null)
        {
            var _host = host;
            return new TcpBack(_host, socket);
        }

        public static TcpBack Init(IPAddress ip, int port, Socket socket = null)
        {
            var _host = new IPEndPoint(ip, port);
            return new TcpBack(_host, socket);
        }

        public static TcpBack Init(string ip, int port, Socket socket = null)
        {
            var _host = new IPEndPoint(IPAddress.Parse(ip), port);
            return new TcpBack(_host, socket);
        }

        public static TcpBack Init(TcpBack back, Socket socket = null)
        {
            var _host = new IPEndPoint(back.host.Address, back.host.Port);
            return new TcpBack(_host, socket);
        }

        public bool Open(int backlog = 1)
        {
            try
            {
                socket.Bind(host);
                socket.Listen(backlog);
                socket.BeginAccept(Accept, null);
                TcpThread.New();
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
            var agent = new TcpAgent(socket.EndAccept(result));
            agent.OnOpen += OnOpenEvent;
            agent.OnClose += OnCloseEvent;
            agent.OnReceive += OnReceiveEvent;
            agent.Init();
            agents.Add(agent);
            socket.BeginAccept(Accept, null);
        }

        public void OnOpen(Action<TcpAgent> action)
        {
            OnOpenEvent += (agent) =>
            {
                TcpThread.Instance?.Add(() =>
                {
                    action?.Invoke(agent);
                });
            };
        }

        public void OnClose(Action<TcpAgent> action)
        {
            OnCloseEvent += (agent) =>
            {
                TcpThread.Instance?.Add(() =>
                {
                    action?.Invoke(agent);
                });
            };
        }

        public void OnReceive(Action<TcpAgent, byte[]> action)
        {
            OnReceiveEvent += (agent, data) =>
            {
                TcpThread.Instance?.Add(() =>
                {
                    action?.Invoke(agent, data);
                });
            };
        }
    }
}