namespace CityExplorerServer
{
    public class CommunityFabric : ISerializableFabric
    {
        public IStringSerializable Create()
        {
            return new Community();
        }
    }
}