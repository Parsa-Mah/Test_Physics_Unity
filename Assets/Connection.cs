using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using gamingCloud.Network.tcp;
using Newtonsoft.Json.Converters;

public class Connection : MonoBehaviour
{
    private tcpStreamer tcp;
    // Start is called before the first frame update
    void Start()
    {
        tcp = new tcpStreamer("localhost", 8569, 10*1024*1024);
        tcp.OnConnected += () => { print("Connected"); };
        tcp.OnPacketRecieve += Tcp_OnPacketRecieve;
    }

    private void Tcp_OnPacketRecieve(string recvPacket)
    {
        UnityMainThreadDispatcher.Instance().Enqueue(() =>
        {
            print(recvPacket);
        });
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
