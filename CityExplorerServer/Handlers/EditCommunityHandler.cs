using System.Collections.Generic;
using CityExplorerServer.NetworkSystem;

namespace CityExplorerServer.Handlers
{
    public class EditCommunityHandler : IPacketHandler
    {
        public void Handle(object bindedArgs, PacketStream stream)
        {
            CommunityManager communityManager = (CommunityManager) bindedArgs;
            long idToEdit = stream.ReadLong();
            List<string> newData = stream.ReadStringList();
            communityManager.EditCommunity(idToEdit, newData);
            NetworkManager.SendPacketToAllClients("editCommunityResponse", (idToEdit, newData));
        }

        public string GetHandledPacketName()
        {
            return "editCommunityRequest";
        }
    }
}