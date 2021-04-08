using System;
using System.IO;

namespace CityExplorerServer.Operations
{
    public class RemoveCommunityOperation : IServerOperation<CommunityManager>
    {
        public void Execute(CommunityManager args, Stream stream)
        {
            byte[] buffer = new byte[8];
            stream.Read(buffer, 0, 8);
            args.RemoveCommunity(BitConverter.ToInt64(buffer, 0));
        }
    }
}