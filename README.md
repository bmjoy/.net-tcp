# Unity TCP
TCP connection framework for unity

## Documentation

> ### TcpFront
> - [Docs of TcpFront or TcpClient](TcpFront.md)

> ## TcpBack
> - [Docs of TcpBack or TcpServer](TcpBack.md)

## Examples

> ### TcpClient & TcpServer
> - Example 1
```csharp
using UnityEngine;
using Zeloot.Tcp;

public class TcpTest : MonoBehaviour
{
    private void Start()
    {
        Server();
        Client();
    }

    private void Server()
    {
        var server = TcpServer.Init("127.0.0.1", 7070);
        var open = server.Open();

        if (open)
        {
            print("[SERVER] listen success on: " + server.host);

            server.OnOpen((agent) =>
            {
                print("[SERVER] client open.");
                agent.Send("Hello client");
                //agent.Close();
            });

            server.OnClose((agent) =>
            {
                print("[SERVER] client close.");
            });

            server.OnReceive((agent, data) =>
            {
                agent.Send(":?close");
                print("[SERVER] receive from client [" + agent.socket.RemoteEndPoint + "] data: " + TcpMain.Decode(data));
            });
        }
        else
        {
            print("[SERVER] listen error on: " + server.host);
        }
    }

    private void Client()
    {
        var client = TcpClient.Init("127.0.0.1", 7070);
        var open = client.Open();

        if (open)
        {
            print("[CLIENT] open success");

            client.OnOpen(() =>
            {
                print("[CLIENT] open.");
            });

            client.OnClose(() =>
            {
                print("[CLIENT] close.");
            });

            client.OnReceive((data) =>
            {
                print("[CLIENT] receive: " + TcpMain.Decode(data));

                if (TcpMain.Decode(data).Trim().ToUpper() == ":?close".Trim().ToUpper())
                    client.Close();
                else
                    client.Send("Hello server");

            });
        }
        else
        {
            print("[CLIENT] open error");
        }
    }
}

```
<br>

> ### TcpFront (Client)
> - Example 1
```csharp
Using Zeloot.Tcp;
Using UnityEngine;

public class Example : MonoBehaviour
{   
    private TcpFront client;
    
    private void Start()
    {
        client.Open(out bool error);
        
        if(!error)
        {
            client.OnOpen(Open);
            client.OnReceive(Receive);
            client.Close(Close);
        
            //preview - in progress
            /*
            client.on("EnterPlayer", EnterPlayer);
            client.emit("ExitPlayer", "[{object}, {json}, {here}]");
            */
        }
        else
        {
            print("Error");
        }
    }
    
    private void Open()
    {
        print("Open");
    }
    
    private void Receive(byte[] data)
    {
        print("Receive: " + TcpFront.Decode(data));
    }
    
    private void Close()
    {
        print("Close");
    }
}
```

> - Example 2
```csharp
Using Zeloot.Tcp;
Using UnityEngine;

public class Example : MonoBehaviour
{    
    private void Start()
    {
        var client = TcpFront.Init("127.0.0.1", 8080);
        
        client.Open(out bool error);
        
        if(error)
        {
            print("Error On Connect");
        }
        
        client.OnOpen(() => 
        {
            print("Connected");
            var message = "Hello, ";
            client.Send(message + "Is Internal Thread (Main Thread)");
            client.SendAsync(message + "Is External Thread (Multi Thread)");
            
            var bytes = new byte[] { 10, 20, 30, 40, 50 };
            client.Send(bytes);
            client.SendAsync(bytes);
        });
        
        client.OnReceive((data) => 
        {
            print("Receive: " + TcpFront.Decode(data));
            
            if(client.IsConnected)
            {
                client.Close();
            }
        });
        
        client.OnClose(() => 
        {
            print("Closed");
        });
    }
}
```
