using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace CityExplorerServer.NetworkSystem
{
    public class NetworkThread : INetworkThread
    {
        private const int DELAY_PACKET_HANDLE = 20;
        
        private Stream stream;
        private PacketStream packetStream;
        private StreamString streamString;
        private Queue<(string, object)> sendQueue = new Queue<(string, object)>();
        private bool isConnected;

        public void SetStream(Stream stream)
        {
            this.stream = stream;
            streamString = new StreamString(stream);
            packetStream = new PacketStream(stream);
            stream.ReadTimeout = DELAY_PACKET_HANDLE;
            stream.WriteTimeout = 2000;
        }

        public bool HandleStream()
        {
            if (sendQueue.Count == 0)
            {
                try
                {
                    stream.ReadByte();
                    string packetName = streamString.ReadString();
                    NetworkManager.RecievePacketFromThread(packetName, packetStream);
                }
                catch (IOException) { }
            }
            else
            {
                try
                {
                    packetStream.Write((byte)1);
                    (string, object) paketUnit = sendQueue.Dequeue();
                    packetStream.Write(paketUnit.Item1);
                    NetworkManager.SendPacketToOtherSide(paketUnit.Item1, paketUnit.Item2, packetStream);
                }
                catch (IOException e)
                {
                    Console.WriteLine(e);
                    return false;
                }
            }
            
            Thread.Sleep(DELAY_PACKET_HANDLE);
            return true;
        }

        public void AddToSendQueue(string packetName, object args)
        {
            sendQueue.Enqueue((packetName, args));
        }

        public void OnConnected()
        {
            sendQueue.Clear();
            isConnected = true;
        }

        public void OnDisconnected()
        {
            sendQueue.Clear();
            isConnected = false;
        }

        public bool IsConnected()
        {
            return isConnected;
        }
    }
}