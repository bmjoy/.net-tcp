# Unity TCP
TCP connection framework for unity

## Documentation

> ### TcpFront
> - [Docs of TcpFront or TcpClient](TcpFront.md)

> ## TcpBack
> - [Docs of TcpBack or TcpServer](TcpBack.md)

## Examples

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
            client.on("EnterPlayer", EnterPlater);
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
