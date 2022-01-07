namespace Zeloot.Tcp
{
    public class TcpEvent
    {
        public delegate void Open(TcpAgent agent);
        public delegate void Close(TcpAgent agent);
        public delegate void Receive(TcpAgent agent, byte[] data);
    }
}