using CityExplorerServer.NetworkSystem;

namespace CityExplorerServer.Handlers
{
    public class RemoveCommunityHandler : IPacketHandler
    {
        public void Handle(object bindedArgs, PacketStream stream, INetworkThread thread)
        {
            CommunityManager communityManager = (CommunityManager) bindedArgs;
            long idToRemove = stream.ReadLong();
            communityManager.RemoveCommunity(idToRemove);
            NetworkManager.SendPacketToAllClients("removeCommunityResponse", idToRemove);
        }

        public string GetHandledPacketName()
        {
            return "removeCommunityRequest";
        }
    }
}