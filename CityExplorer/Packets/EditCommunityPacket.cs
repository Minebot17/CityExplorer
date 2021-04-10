using CityExplorerServer.NetworkSystem;

namespace CityExplorer.Packets
{
    public class EditCommunityPacket : IPacket
    {
        public void Write(object bindedArgs, object args, PacketStream stream)
        {
            Community community = (Community) args;
            stream.Write(community.Id);
            stream.Write(community.Serialize());
        }

        public string GetPacketName()
        {
            return "editCommunityRequest";
        }
    }
}