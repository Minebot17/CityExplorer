using System.Collections.Generic;
using System.Linq;
using CityExplorerServer.NetworkSystem;

namespace CityExplorer.Handlers
{
    public class EditCommunityHandler : IPacketHandler
    {
        public void Handle(object bindedArgs, PacketStream stream, INetworkThread thread)
        {
            ApplicationViewModel viewModel = (ApplicationViewModel) bindedArgs;
            long idToEdit = stream.ReadLong();
            List<string> newData = stream.ReadStringList();
            Community editedCommunity = viewModel.Communities.FirstOrDefault(c => c.Id == idToEdit);
            editedCommunity?.Deserialize(newData);
        }

        public string GetHandledPacketName()
        {
            return "editCommunityResponse";
        }
    }
}