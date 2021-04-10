using System.Linq;
using CityExplorerServer.NetworkSystem;

namespace CityExplorer.Handlers
{
    public class RemoveCommunityHandler : IPacketHandler
    {
        public void Handle(object bindedArgs, PacketStream stream, INetworkThread thread)
        {
            ApplicationViewModel viewModel = (ApplicationViewModel) bindedArgs;
            long idToRemove = stream.ReadLong();
            Community toRemove = viewModel.Communities.FirstOrDefault(c => c.Id == idToRemove);

            if (toRemove != null)
                viewModel.Communities.Remove(toRemove);
        }

        public string GetHandledPacketName()
        {
            return "removeCommunityResponse";
        }
    }
}