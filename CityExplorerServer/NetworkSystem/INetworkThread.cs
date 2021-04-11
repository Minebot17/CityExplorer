﻿using System.IO;

namespace CityExplorerServer.NetworkSystem
{
    public interface INetworkThread
    {
        void SetStream(Stream stream);
        bool HandleStreamWrite(); // if false - disconect
        bool HandleStreamRead(); // if false - disconect
        void AddToSendQueue(string packetName, object args);
        
        void OnConnected();
        void OnDisconnected();
        bool IsConnected();
    }
}