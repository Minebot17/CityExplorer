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
        private const int BUFFER_DATA_COUNT = 10240;
        
        private Stream stream;
        private StreamString streamString;
        private byte[] buffer = new byte[8];
        private byte[] memoryBuffer;
        private MemoryStream memoryStream;
        private StreamString memoryStreamString;
        private int dataSize;
        private Random rnd;

        public PacketStream(Stream stream)
        {
            this.stream = stream;
            memoryBuffer = new byte[BUFFER_DATA_COUNT];
            memoryStream = new MemoryStream(memoryBuffer);
            streamString = new StreamString(stream);
            memoryStreamString = new StreamString(memoryStream);
            rnd = new Random();
        }

        public bool DataIsOver()
        {
            return memoryStream.Position >= dataSize;
        }

        public bool ReadByte(int timeout)
        {
            /*CancellationTokenSource tokenSource = new CancellationTokenSource();
            Task<int> readTask = stream.ReadAsync(buffer, 0, 1, tokenSource.Token);
            Thread.Sleep(timeout);

            if (!readTask.IsCompleted)
            {
                tokenSource.Cancel();
                Thread.Sleep(timeout / 3);
                readTask.Dispose();
                return false;
            }

            if (readTask.Exception != null)
            {
                Trace.WriteLine("exception");
                readTask.Dispose();
                return false;
            }

            try
            {
                readTask.Dispose();
                return readTask.Result == 1;
            }
            catch (Exception e)
            {
                Trace.WriteLine(e);
                return false;
            }*/

            /*int result = 0;
            Thread thread = new Thread(() =>
            {
                result = stream.Read(buffer, 0, 1);
            });
            thread.Start();
            Thread.Sleep(timeout);

            bool receive = result == 1;
            if (!receive)
                thread.Abort();

            return receive;*/
            
            int result = 0;
            IAsyncResult asyncResult = stream.BeginRead(buffer, 0, 1, ar => { result = 1; }, rnd.Next());
            Thread.Sleep(timeout);
            stream.EndRead(asyncResult);
            return result == 1;
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