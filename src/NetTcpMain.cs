using System;
using System.Collections.Generic;
using System.Linq;
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

        public static class NetStream
        {
            public static byte[] Encode(string name, string data)
            {
                return Encode(Encoding.UTF8.GetBytes(name), Encoding.UTF8.GetBytes(data));
            }

            public static byte[] Encode(string name, byte[] data)
            {
                return Encode(Encoding.UTF8.GetBytes(name), data);
            }

            public static byte[] Encode(byte[] name, string data)
            {
                return Encode(name, Encoding.UTF8.GetBytes(data));
            }

            private static void GetName(byte[] name, out string text, out byte[] bytes)
            {
                GetName(Encoding.UTF8.GetString(name), out text, out bytes);
            }

            private static void GetName(string name, out string text, out byte[] bytes)
            {
                var nameText = $"%%{name}%%";
                var nameByte = Encoding.UTF8.GetBytes(nameText);
                text = nameText;
                bytes = nameByte;
            }

            public static byte[] Encode(byte[] name, byte[] data)
            {
                GetName(name, out var nameText, out var nameByte);

                var buffer = new List<byte[]>();
                buffer.Add(nameByte);
                buffer.Add(data);
                return buffer.SelectMany(a => a).ToArray();
            }

            public static byte[] Decode(string name, byte[] buffer)
            {
                GetName(name, out var nameText, out var nameByte);

                if (!(buffer.Length >= nameByte.Length)) return null;

                var nameByteDecode = new byte[nameByte.Length];
                Array.Copy(buffer, nameByteDecode, nameByteDecode.Length);
                var nameTextDecode = Encoding.UTF8.GetString(nameByteDecode);

                if (nameText != nameTextDecode) return null;

                var data = new byte[buffer.Length - nameByteDecode.Length];
                /*
                UnityEngine.Debug.Log("value: " + nameByte.Length);
                UnityEngine.Debug.Log("buffer: " + buffer.Length);
                UnityEngine.Debug.Log("result: " + data.Length);
                */
                Array.Copy(buffer, nameByte.Length, data, 0, data.Length);
                return data;
            }
        }
    }
}