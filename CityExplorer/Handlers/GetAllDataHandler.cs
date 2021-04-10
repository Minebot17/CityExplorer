using System.Collections.Generic;
using CityExplorerServer.NetworkSystem;

namespace CityExplorer.Handlers
{
    public class GetAllDataHandler : IPacketHandler
    {
        public void Handle(object bindedArgs, PacketStream stream, INetworkThread thread)
        {
            ApplicationViewModel viewModel = (ApplicationViewModel) bindedArgs;
            int count = stream.ReadInt();

            for (int i = 0; i < count; i++)
            {
                Community community = new Community(viewModel);
                community.Deserialize(stream.ReadStringList());
                viewModel.Communities.Add(community);
            }
        }

        public string GetHandledPacketName()
        {
            return "getAllDataResponse";
        }
    }
}