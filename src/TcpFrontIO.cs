using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

namespace Zeloot.Tcp
{
    public class TcpFrontIO
    {

        public void open(string ip, int port)
        {

        }

        public void open(IPAddress address, int port)
        {

        }

        public void open(IPEndPoint host)
        {

        }

        public void emit(string name, byte[] data)
        {

        }

        public void emit(string name, string data)
        {

        }

        public void on(string name, Action<DataIO> data)
        {

        }
    }
}