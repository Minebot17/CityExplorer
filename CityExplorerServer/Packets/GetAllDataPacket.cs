using CityExplorerServer.NetworkSystem;

namespace CityExplorerServer.Packets
{
    public class GetAllDataPacket : IPacket
    {
        public void Write(object bindedArgs, object args, PacketStream stream)
        {
            CommunityManager communityManager = (CommunityManager) args;
            stream.Write(communityManager.Communities.Count);

            foreach (Community community in communityManager.Communities)
                stream.Write(community.Serialize());
        }

        public string GetPacketName()
        {
            return "getAllDataResponse";
        }
    }
}