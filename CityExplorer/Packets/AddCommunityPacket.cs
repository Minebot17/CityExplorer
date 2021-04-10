using CityExplorerServer.NetworkSystem;

namespace CityExplorer.Packets
{
    public class AddCommunityPacket : IPacket
    {
        public void Write(object bindedArgs, object args, PacketStream stream)
        {
            
        }

        public string GetPacketName()
        {
            return "addCommunityRequest";
        }
    }
}