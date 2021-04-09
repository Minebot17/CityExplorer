using System.IO;

namespace CityExplorerServer.NetworkSystem
{
    public interface INetworkThread
    {
        void SetStream(Stream stream);
        bool HandleStream(); // if false - disconect
        void AddToSendQueue(string packetName, object args);
        
        void OnConnected();
        void OnDisconnected();
        bool IsConnected();
    }
}