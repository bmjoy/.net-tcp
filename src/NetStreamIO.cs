using System;

namespace Zeloot.Tcp
{
    public class NetStreamIO
    {
        internal readonly NetTcpServerAgent agent;
        public readonly string uid;
        private bool init;
        internal bool IsConnected
        {
            get
            {
                if (agent == null || agent.socket == null) return false;
                return agent.IsConnected;
            }
        }

        public NetStreamIO(NetTcpServerAgent client)
        {
            this.agent = client;
            this.init = false;
            this.uid = Guid.NewGuid().ToString();
        }
        public void On(string name, Action action)
        {
            if (name == null) return;

            if (name == "open")
            {
                agent.OnOpen += (_) =>
                {
                    action?.Invoke();
                };

                if (!init) action?.Invoke();
            }
            else if (name == "close")
            {
                agent.OnClose += (_) =>
                {
                    action?.Invoke();
                };
            }
        }

        public void On(string name, Action<byte[]> action)
        {
            if (name == null) return;

            agent.OnReceive += (_, data) =>
            {
                var buffer = NetTcpMain.NetStream.Decode(name, data);
                if (buffer == null) return;
                action?.Invoke(buffer);
            };
        }

        public void Emit(string name, byte[] data, bool async = true)
        {
            if (async)
            {
                agent.SendAsync(NetTcpMain.NetStream.Encode(name, data));
            }
            else
            {
                agent.Send(NetTcpMain.NetStream.Encode(name, data));
            }
        }

        public void Emit(string name, string data, bool async = true)
        {
            if (async)
            {
                agent.SendAsync(NetTcpMain.NetStream.Encode(name, data));
            }
            else
            {
                agent.Send(NetTcpMain.NetStream.Encode(name, data));
            }
        }

        public void Close()
        {
            agent?.Close();
        }
    }
}