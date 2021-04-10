using CityExplorerServer.NetworkSystem;

namespace CityExplorerServer.Handlers
{
    public class GetAllDataHandler : IPacketHandler
    {
        public void Handle(object bindedArgs, PacketStream stream, INetworkThread thread)
        {
            NetworkManager.SendPacketToClient("getAllDataResponse", bindedArgs, thread);
        }

        public string GetHandledPacketName()
        {
            return "getAllDataRequest";
        }
    }
}