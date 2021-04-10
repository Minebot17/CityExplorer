using System.Collections.Generic;
using CityExplorerServer.NetworkSystem;

namespace CityExplorerServer.Packets
{
    public class AddCommunityPacket : IPacket
    {
        public void Write(object bindedArgs, object args, PacketStream stream)
        {
            Community community = (Community) args;
            stream.Write(community.Serialize());
        }

        public string GetPacketName()
        {
            return "addCommunityResponse";
        }
    }
}