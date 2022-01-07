using System;

namespace Zeloot.Tcp
{
    public class DataIO
    {
        public readonly string text;
        public readonly byte[] bytes;

        private DataIO(byte[] buffer)
        {
            bytes = buffer;
            text = TcpMain.Decode(buffer);
        }

        public static DataIO Init(byte[] data, int offset)
        {
            var buffer = new byte[data.Length - offset];
            Array.Copy(data, offset, buffer, 0, buffer.Length);
            return new DataIO(buffer);
        }
    }
}