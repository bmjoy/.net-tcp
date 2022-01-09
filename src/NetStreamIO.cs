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
#if UNITY_STANDALONE || UNITY_IOS || UNITY_WII || UNITY_ANDROID || UNITY_PS4 || UNITY_XBOXONE || UNITY_LUMIN || UNITY_TIZEN || UNITY_TVOS || UNITY_WEBGL || UNITY_ANALYTICS || UNITY_WINRT

                    NetMainThread.Instance?.Add(() =>
                    {
                        action?.Invoke();
                    });
#else
                        action?.Invoke();
#endif
                };

#if UNITY_STANDALONE || UNITY_IOS || UNITY_WII || UNITY_ANDROID || UNITY_PS4 || UNITY_XBOXONE || UNITY_LUMIN || UNITY_TIZEN || UNITY_TVOS || UNITY_WEBGL || UNITY_ANALYTICS || UNITY_WINRT

                NetMainThread.Instance?.Add(() =>
                {
                    if (!init) action?.Invoke();
                });
#else
                    if (!init) action?.Invoke();
#endif
            }
            else if (name == "close")
            {
                agent.OnClose += (_) =>
                {
#if UNITY_STANDALONE || UNITY_IOS || UNITY_WII || UNITY_ANDROID || UNITY_PS4 || UNITY_XBOXONE || UNITY_LUMIN || UNITY_TIZEN || UNITY_TVOS || UNITY_WEBGL || UNITY_ANALYTICS || UNITY_WINRT

                    NetMainThread.Instance?.Add(() =>
                    {
                        action?.Invoke();
                    });
#else
                        action?.Invoke();
#endif
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

#if UNITY_STANDALONE || UNITY_IOS || UNITY_WII || UNITY_ANDROID || UNITY_PS4 || UNITY_XBOXONE || UNITY_LUMIN || UNITY_TIZEN || UNITY_TVOS || UNITY_WEBGL || UNITY_ANALYTICS || UNITY_WINRT

                NetMainThread.Instance?.Add(() =>
                {
                    action?.Invoke(buffer);
                });
#else
                    action?.Invoke(buffer);
#endif
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