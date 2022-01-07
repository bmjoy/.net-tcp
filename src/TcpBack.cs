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


        private TcpBack(ref IPEndPoint host, ref Socket socket)
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
            return new TcpBack(ref _host, ref socket);
        }

        public static TcpBack Init(IPAddress ip, int port, Socket socket = null)
        {
            var _host = new IPEndPoint(ip, port);
            return new TcpBack(ref _host, ref socket);
        }

        public static TcpBack Init(string ip, int port, Socket socket = null)
        {
            var _host = new IPEndPoint(IPAddress.Parse(ip), port);
            return new TcpBack(ref _host, ref socket);
        }

        public static TcpBack Init(TcpBack back, Socket socket = null)
        {
            var _host = new IPEndPoint(back.host.Address, back.host.Port);
            return new TcpBack(ref _host, ref socket);
        }

        public bool Open(int backlog = 1)
        {
            try
            {
                socket.Bind(host);
                socket.Listen(backlog);
                socket.BeginAccept(Accept, null);
                IsListen = true;
                return true;
            }
            catch
            {
                IsListen = false;
                return false;
            }
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
            var client = socket.EndAccept(result);
            var agent = new TcpAgent(client);
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
                action?.Invoke(agent);
            };
        }

        public void OnClose(Action<TcpAgent> action)
        {
            OnCloseEvent += (agent) =>
            {
                action?.Invoke(agent);
            };
        }

        public void OnReceive(Action<TcpAgent, byte[]> action)
        {
            OnReceiveEvent += (agent, data) =>
            {
                action?.Invoke(agent, data);
            };
        }
    }
}