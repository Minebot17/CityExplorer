namespace CityExplorerServer.NetworkSystem
{
    public interface IPacket
    {
        void Write(object bindedArgs, object args, PacketStream stream);
        string GetPacketName();
    }
}