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

public class Example
{    
    private void Start()
    {
        var client = TcpFront.Init("127.0.0.1", 8080);
        client.Open();
        
        client.OnOpen(() => 
        {
            print("Connected");
            client.Send("Hello, Is Internal Thread");
            client.SendAsync("Hello, External Thread");
        });
        
        client.OnReceive((data), => 
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
