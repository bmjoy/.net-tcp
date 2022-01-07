using System.Text;

namespace Zeloot.Tcp
{
    public class NetTcpMain
    {
        public static string Decode(byte[] data)
        {
            return Encoding.UTF8.GetString(data);
        }

        public static byte[] Encode(string data)
        {
            return Encoding.UTF8.GetBytes(data);
        }
    }
}