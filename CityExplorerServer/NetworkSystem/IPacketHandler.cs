namespace CityExplorerServer.NetworkSystem
{
    public interface IPacketHandler
    {
        void Handle(object bindedArgs, PacketStream stream, INetworkThread thread);
        string GetHandledPacketName();
    }
}