using CityExplorerServer.NetworkSystem;

namespace CityExplorerServer.Handlers
{
    public class AddCommunityHandler : IPacketHandler
    {
        public void Handle(object bindedArgs, PacketStream stream)
        {
            CommunityManager communityManager = (CommunityManager) bindedArgs;
            Community addedCommunity = communityManager.AddNewCommunity();
            NetworkManager.SendPacketToAllClients("addCommunityResponse", addedCommunity);
        }

        public string GetHandledPacketName()
        {
            return "addCommunityRequest";
        }
    }
}