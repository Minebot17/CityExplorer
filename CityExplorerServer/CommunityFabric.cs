namespace CityExplorerServer
{
    public class CommunityFabric : ISerializableFabric<Community>
    {
        public Community Create()
        {
            return new Community();
        }
    }
}