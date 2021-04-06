using System.Collections.Generic;

namespace CityExplorerServer
{
    public class ServerManager
    {
        private IDataSerializer serializer;

        public ServerManager(IDataSerializer serializer)
        {
            this.serializer = serializer;
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
    }
}