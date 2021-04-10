using CityExplorerServer.NetworkSystem;

namespace CityExplorer.Packets
{
    public class GetAllDataPacket : IPacket
    {
        public void Write(object bindedArgs, object args, PacketStream stream)
        {
            
        }

        public string GetPacketName()
        {
            return "getAllDataRequest";
        }
    }
}