using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace CityExplorerServer.NetworkSystem
{
    public class PacketStream
    {
        private const int BUFFER_DATA_COUNT = 10240;
        
        private Stream stream;
        private StreamString streamString;
        private byte[] buffer = new byte[8];
        private byte[] memoryBuffer;
        private MemoryStream memoryStream;
        private StreamString memoryStreamString;
        private int dataSize;
        private CancellationTokenSource tokenSource;

        public PacketStream(Stream stream)
        {
            this.stream = stream;
            memoryBuffer = new byte[BUFFER_DATA_COUNT];
            memoryStream = new MemoryStream(memoryBuffer);
            streamString = new StreamString(stream);
            memoryStreamString = new StreamString(memoryStream);
            tokenSource = new CancellationTokenSource();
        }

        public void CollectData(int collectTime)
        {
            memoryStream.Seek(0, SeekOrigin.Begin);
            Task<int> readTask = stream.ReadAsync(memoryBuffer, 0, BUFFER_DATA_COUNT, tokenSource.Token);
            Thread.Sleep(collectTime);
            tokenSource.Cancel();
            Thread.Sleep(collectTime/5);
            
            if (readTask.IsCanceled)
                readTask.Dispose();
            
            dataSize = readTask.Result;
        }

        public bool DataIsOver()
        {
            return memoryStream.Position >= dataSize;
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
            StreamString streamString = DataIsOver() ? this.streamString : memoryStreamString;
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