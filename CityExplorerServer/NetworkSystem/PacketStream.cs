using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace CityExplorerServer.NetworkSystem
{
    public class PacketStream
    {
        private Stream stream;
        private StreamString streamString;
        private byte[] buffer = new byte[8];

        public PacketStream(Stream stream)
        {
            this.stream = stream;
            streamString = new StreamString(stream);
        }

        public void Flush()
        {
            stream.Flush();
        }

        public void Write(int value)
        {
            byte[] buffer = BitConverter.GetBytes(value);
            stream.Write(buffer, 0, 4);
        }

        public void Write(long value)
        {
            byte[] buffer = BitConverter.GetBytes(value);
            stream.Write(buffer, 0, 8);
        }

        public void Write(string value)
        {
            StreamString streamString = new StreamString(stream);
            streamString.WriteString(value);
        }

        public void Write(List<string> value)
        {
            Write(value.Count);
            foreach (string s in value)
                Write(s);
        }

        public int ReadInt()
        {
            stream.Read(buffer, 0, 4);
            return BitConverter.ToInt32(buffer, 0);
        }
        
        public long ReadLong()
        {
            stream.Read(buffer, 0, 8);
            return BitConverter.ToInt64(buffer, 0);
        }
        
        public string ReadString()
        {
            return streamString.ReadString();
        }

        public List<string> ReadStringList()
        {
            int count = ReadInt();
            List<string> result = new List<string>();
            for (int i = 0; i < count; i++)
                result.Add(ReadString());

            return result;
        }
    }
}