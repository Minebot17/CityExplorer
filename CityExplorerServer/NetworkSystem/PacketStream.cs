using System;
using System.IO;

namespace CityExplorerServer.NetworkSystem
{
    public class PacketStream
    {
        private Stream stream;
        private byte[] buffer = new byte[8];

        public PacketStream(Stream stream)
        {
            this.stream = stream;
        }

        public void Write<T>(T value)
        {
            if (value is int a)
                WriteInt(a);
            else if (value is long b)
                WriteLong(b);
            else if (value is string c)
                WriteString(c);
            else if (value is byte d)
                WriteByte(d);
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

        private void WriteByte(byte value)
        {
            stream.WriteByte(value);
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
            StreamString streamString = new StreamString(stream);
            return streamString.ReadString();
        }
    }
}