using CityExplorerServer.NetworkSystem;

namespace CityExplorer.Handlers
{
    public class AddCommuntiyHandler : IPacketHandler
    {
        public void Handle(object bindedArgs, PacketStream stream)
        {
            ApplicationViewModel viewModel = (ApplicationViewModel) bindedArgs;
            Community newCommunity = new Community();
            newCommunity.Deserialize(stream.ReadStringList());
            viewModel.Communities.Add(newCommunity);
        }

        public string GetHandledPacketName()
        {
            return "addCommunityResponse";
        }
    }
}