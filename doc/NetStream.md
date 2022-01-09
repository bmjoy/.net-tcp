> ## NetStream *Server

## Docs
[`Examples`](#examples)
> ### Namespace
 ```csharp
Using Zeloot.Tcp;
```

 ## `examples`

```csharp

using UnityEngine;
using Zeloot.Tcp;

public class Example : MonoBehaviour
{
    private void Start()
    {
        var stream = new NetStream("127.0.0.1", 12000);

        stream.On("open", () =>
        {
            print("stream open");
        });

        stream.On("close", () =>
        {
            print("stream close");
        });

        stream.On("connection", (io) =>
        {
            io.On("open", () =>
            {
                print("client connected");

                io.Emit("ping", "message");

                stream.Broadcast("player open", "uid: " + io.uid);
            });

            io.On("close", () =>
            {
                print("client closed");

                stream.Broadcast("player close", "uid: " + io.uid);
            });

            io.On("ping", (data) =>         /* my event */
            {
                print("client receive ping\n" + NetTcpMain.Decode(data));
            });
            
            io.On("close-me", (data) =>     /* my event */
            {
                io.Emit("ping", new byte[] { 0, 1, 2, 4, 5, 6, 7, 8, 9 });
                
                io.Close();
            });
        });

        stream.Open();
    }
}
```
