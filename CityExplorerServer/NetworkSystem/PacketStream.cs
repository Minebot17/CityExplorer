using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace CityExplorerServer.NetworkSystem
{
    public class PacketStream
    {
        private const int BUFFER_DATA_COUNT = 10240;
        
        private Stream stream;
        private byte[] buffer = new byte[8];
        private MemoryStream memoryStream;
        private int dataSize;

        public PacketStream(Stream stream)
        {
            this.stream = stream;
            memoryStream = new MemoryStream(new byte[BUFFER_DATA_COUNT]);
            
        }

        public void CollectData(int collectTime)
        {
            memoryStream.Seek(0, SeekOrigin.Begin);
            Task<int> readTask = stream.ReadAsync(memoryStream.GetBuffer(), 0, BUFFER_DATA_COUNT);
            Thread.Sleep(collectTime);
            dataSize = readTask.Result;
            readTask.Dispose();
        }

        public bool DataIsOver()
        {
            return memoryStream.Position >= dataSize;
        }

        public void Flush()
        {
            stream.Flush();
        }

        public void Write<T>(T value)
        {
            if (value is int a)
                WriteInt(a);
            else if (value is long b)
                WriteLong(b);
            else if (value is string c)
                WriteString(c);
        }
        
        private void WriteInt(int value)
        {
            byte[] buffer = BitConverter.GetBytes(value);
            stream.Write(buffer, 0, 4);
        }

        private void WriteLong(long value)
        {
            byte[] buffer = BitConverter.GetBytes(value);
            stream.Write(buffer, 0, 8);
        }

        private void WriteString(string value)
        {
            StreamString streamString = new StreamString(stream);
            streamString.WriteString(value);
        }

        public int ReadInt()
        {
            Stream actualStream = DataIsOver() ? stream : memoryStream;
            actualStream.Read(buffer, 0, 4);
            return BitConverter.ToInt32(buffer, 0);
        }
        
        public long ReadLong()
        {
            Stream actualStream = DataIsOver() ? stream : memoryStream;
            actualStream.Read(buffer, 0, 8);
            return BitConverter.ToInt64(buffer, 0);
        }
        
        public string ReadString()
        {
            Stream actualStream = DataIsOver() ? stream : memoryStream;
            StreamString streamString = new StreamString(actualStream);
            return streamString.ReadString();
        }
    }
}