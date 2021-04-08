using System;
using System.IO;

namespace CityExplorerServer.Operations
{
    public class AddCommunityOperation : IServerOperation<CommunityManager>
    {
        public void Execute(CommunityManager args, Stream stream)
        {
            long communityId = args.AddNewCommunity();
            stream.Write(BitConverter.GetBytes(communityId), 0, 8);
        }
    }
}