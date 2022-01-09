> ## NetStreamClient *Client

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
    NetStreamClient stream = new NetStreamClient("127.0.0.1", 12000);

    private void Start()
    {
        stream.On("open", () =>
        {
            print("stream open");
            stream.Emit("ping", "Hello server!");
        });

        stream.On("close", () =>
        {
            print("stream close");
        });

        stream.On("ping", (data) =>         /* my event */
        {
            //send byte
            stream.Emit("pong", new byte[2] { 1, 2 });

            //send text
            stream.Emit("pong", "json or string here");
            print("receive ping\n\t" + NetTcpMain.Decode(data));
        });

        stream.On("pong", (data) =>         /* my event */
        {
            //send byte
            stream.Emit("pong", new byte[2] { 1, 2 });

            //send text
            stream.Emit("pong", "json or string here");
            print("receive ping\n\t" + NetTcpMain.Decode(data));
        });

        stream.On("close-me", (data) =>      /* my event */
        {
            stream.Close();
        });

        stream.On("create object", (data) =>
        {
            print(NetTcpMain.Decode(data));
#if UNITY_EDITOR
            var player = new GameObject(NetTcpMain.Decode(data));
#endif
        });

        stream.Open();
    }
#if UNITY_EDITOR
    private void LateUpdate()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            stream.Emit("ping", " ");
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            stream.Emit("pong", " ");
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            stream.Emit("create object", "Object: " + "001");
        }
    }
#endif
}

```