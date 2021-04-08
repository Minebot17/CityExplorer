using System.Collections.Generic;
using System.Linq;

namespace CityExplorerServer
{
    public class CommunityManager
    {
        private IDataSerializer serializer;
        private CommunityFabric fabric;
        private List<Community> communities;

        public CommunityManager(IDataSerializer serializer)
        {
            this.serializer = serializer;
            fabric = new CommunityFabric();
            communities = serializer.Load(fabric);
        }

        public long AddNewCommunity()
        {
            lock (communities)
            {
                Community newCommunity = fabric.Create();
                communities.Add(newCommunity);
                return newCommunity.Id;
            }
        }

        public void RemoveCommunity(long id)
        {
            lock (communities)
            {
                communities.RemoveAll(c => c.Id == id);
            }
        }

        public void EditCommunity(long id, List<string> data)
        {
            lock (communities)
            {
                communities.FirstOrDefault(c => c.Id == id)?.Deserialize(data);
            }
        }

        public void SerializeAllData()
        {
            lock (communities)
            {
                serializer.Save(communities);
            }
        }
    }
}