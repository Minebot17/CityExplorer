using System.Collections.Generic;
using CityExplorerServer.NetworkSystem;

namespace CityExplorerServer.Packets
{
    public class EditCommunityPacket : IPacket
    {
        public void Write(object bindedArgs, object args, PacketStream stream)
        {
            (long, List<string>) pair = ((long, List<string>)) args;
            stream.Write(pair.Item1);
            stream.Write(pair.Item2);
        }

        public string GetPacketName()
        {
            return "editCommunityResponse";
        }
    }
}