using CityExplorerServer.NetworkSystem;

namespace CityExplorer.Packets
{
    public class RemoveCommunityPacket : IPacket
    {
        public void Write(object bindedArgs, object args, PacketStream stream)
        {
            long idToRemove = (long) args;
            stream.Write(idToRemove);
        }

        public string GetPacketName()
        {
            return "removeCommunityRequest";
        }
    }
}