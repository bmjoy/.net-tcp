> ## docs of NetTcpClient
> ### Docs
>   - Namespace: Using Zeloot.Tcp;
>   - class: NetTcpClient
>   - Properties:
>       - bool IsConnected
>       - Socket socket
>       - IPEndPoint host
>   - Methods
>       - bool Open()
>       - void Close()
>       - void Send(byte[] data)
>       - void Send(string data)
>       - void SendAsync(byte[] data)
>       - void SendAsync(string data)
>       - void Receive(Action<byte[]> action)
>       - Events
>           - void OnOpen(Action action)
>           - void OnClose(Action action)
>           - void OnReceive(Action<byte[]> action)