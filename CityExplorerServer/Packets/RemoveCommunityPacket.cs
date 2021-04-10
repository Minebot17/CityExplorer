using CityExplorerServer.NetworkSystem;

namespace CityExplorerServer.Packets
{
    public class RemoveCommunityPacket : IPacket
    {
        public void Write(object bindedArgs, object args, PacketStream stream)
        {
            stream.Write((long) args);
        }

        public string GetPacketName()
        {
            return "removeCommunityResponse";
        }
    }
}