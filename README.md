# Unity TCP
TCP connection framework for unity

## Documentation

> ### TcpFront
<br>

> namespace
```csharp
Using Zeloot.Tcp;
```
<br>

> Instance
```csharp
client = TcpFront.Init(ip, port);
```

> ## Methods
<br>

> - Init
```
work    :  Used for init TcpFront.
option.1 param :  <IPEndPoint host, [?] Socket socket>
option.2 param :  <string ip, int port, [?] Socket socket>
option.3 param :  <IPAddress address, int port, [?] Socket socket>
option.4 param :  <TcpFront front, [?] Socket socket>
```
```csharp
TcpFront front = TcpFront.Init("127.0.0.1", 8080);
```

> - Open
```
work    :  Used for open connection.
param.1 :  bool error  | Return true if have error and false if not have error.
param.2 :  int timeout | Is max-time for try open connection [Default: 1000 ~ 1s].
```
```csharp
front.Open();
```

> - Close
```
work    :  Used for close connection.
```
```csharp
front.Close();
```

> - Send / SendAsync
```
work    :  Used for send message or data.
param.1 :  Accept, string or array of bytes
```
```csharp
front.Send("message");
front.SendAsync("message");
front.Send(new byte[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 });
front.SendAsync(new byte[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 });
```

> ## Events
<br>

> ### OnOpen / OnClose
```
work    :  Used for receive events.
param.1 :  Accept, Action 'System.Action'.
```
> Using lambda
```csharp
front.OnOpen(() => {
    Debug.Log("On Open");
});

front.OnClose(() => {
    Debug.Log("On Close");
});
```
> Using function
```csharp
front.OnOpen(MyOnOpen);
front.OnClose(MyOnClose);

void MyOnOpen() { Debug.Log("My On Open"); }
void MyOnClose() { Debug.Log("My On Close"); }
```

> ### OnReceive
```
work    :  Used for receive events.
param.1 :  Accept, Action<byte[]> 'System.Action'.
```
> Using lambda
```csharp
front.OnReceive((data) => {
    Debug.Log("On Receive: " + TcpFront.Decode(data));
});
```
> Using function
```csharp
front.OnReceive(MyOnReceive);

void MyOnReceive(byte[] data) { Debug.Log("My On Open: " + TcpFront.Decode(data));
```

<br>

> Example
```csharp
using UnityEngine;
using Zeloot.Tcp;

public class TcpFrontExample : MonoBehaviour
{
    private TcpFront client;

    private void Start()
    {
        //Init client
        client = TcpFront.Init("127.0.0.1", 8080);

        //Open connection
        client.Open(out bool error);

        //OnConnect
        client.OnOpen(() =>
        {
            Debug.Log("On Open");
        });

        //OnReceive
        client.OnReceive((data) =>
        {
            Debug.Log("On Receive: " + TcpFront.Decode(data));
        });

        //OnClose
        client.OnClose(() =>
        {
            Debug.Log("On Close");
        });

        //Extra | validate if the connection was opened
        if (error)
            Debug.LogError("Have error on connect");
        else
            Debug.LogWarning("Client connected");
    }

    private void OnApplicationQuit()
    {
        client?.Close();
    }
}
```
