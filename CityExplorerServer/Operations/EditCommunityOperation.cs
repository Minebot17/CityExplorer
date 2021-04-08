using System;
using System.Collections.Generic;
using System.IO;

namespace CityExplorerServer.Operations
{
    public class EditCommunityOperation : IServerOperation<CommunityManager>
    {
        public void Execute(CommunityManager args, Stream stream)
        {
            byte[] buffer = new byte[8];
            
            stream.Read(buffer, 0, 8);
            long idtoEdit = BitConverter.ToInt64(buffer, 0);

            stream.Read(buffer, 0, 4);
            long stringsCount = BitConverter.ToInt32(buffer, 0);

            List<string> data = new List<string>();
            StreamString streamString = new StreamString(stream);
            for (int i = 0; i < stringsCount; i++)
                data.Add(streamString.ReadString());
            
            args.EditCommunity(idtoEdit, data);
        }
    }
}