namespace Zeloot.Tcp
{
    public class TcpEvent
    {
        public delegate void Open(TcpServerAgent agent);
        public delegate void Close(TcpServerAgent agent);
        public delegate void Receive(TcpServerAgent agent, byte[] data);
    }
}