namespace Zeloot.Tcp
{
    public class NetTcpEvent
    {
        public delegate void Open(NetTcpServerAgent agent);
        public delegate void Close(NetTcpServerAgent agent);
        public delegate void Receive(NetTcpServerAgent agent, byte[] data);
    }
}