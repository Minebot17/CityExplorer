using System.Collections.Generic;

namespace CityExplorerServer
{
    public class ServerManager
    {
        private IDataSerializer serializer;
        private List<Community> communities;

        public ServerManager(IDataSerializer serializer)
        {
            this.serializer = serializer;
            communities = serializer.Load(new CommunityFabric());
        }

        public void AddNewCommunity()
        {
            // TODO
        }

        public void RemoveCommunity(long id)
        {
            // TODO
        }

        public void EditCommunity(List<string> data)
        {
            // TODO
        }

        public void SerializeAllData()
        {
            serializer.Save(communities);
        }
    }
}