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

> Methods
<br>

> - Open
```
work    :  Used for open connection.
param.1 :  bool error  | Return true if have error and false if not have error.
param.2 :  int timeout | Is max-time for try open connection [Default: 1000 ~ 1s] 
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
