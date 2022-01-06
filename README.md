# Unity TCP
TCP connection framework for unity

## Documentation

> ### TcpFront
> - [Docs of TcpFront or TcpClient](TcpFront.md)

> ## TcpBack
> - [Docs of TcpBack or TcpServer](TcpBack.md)

## Examples

> ### TcpFront (Client)
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
            client.Close();
        });
        
        client.OnClose(() => 
        {
            print("Closed");
        });
    }
}
```
