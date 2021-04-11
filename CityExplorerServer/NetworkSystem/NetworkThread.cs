using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace CityExplorerServer.NetworkSystem
{
    public class NetworkThread : INetworkThread
    {
        private const int DELAY_PACKET_HANDLE = 50;
        
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
        }

        public bool HandleStream()
        {
            if (sendQueue.Count == 0)
            {
                Trace.WriteLine("startObserve");
                bool revievePacket = packetStream.ReadByte(DELAY_PACKET_HANDLE);
                Trace.WriteLine("endObserve");
                
                if (revievePacket)
                {
                    Trace.WriteLine("startRecieve");
                    string packetName = streamString.ReadString();
                    NetworkManager.RecievePacketFromThread(packetName, packetStream, this);
                    Trace.WriteLine("endRecieve");
                }
            }
            else
            {
                Trace.WriteLine("startSend");
                try
                {
                    stream.Write(new byte[1], 0, 1);
                    (string, object) paketUnit = sendQueue.Dequeue();
                    packetStream.Write(paketUnit.Item1);
                    NetworkManager.SendPacketToOtherSide(paketUnit.Item1, paketUnit.Item2, packetStream);
                    packetStream.Flush();
                }
                catch (IOException e)
                {
                    Trace.WriteLine(e);
                    return false;
                }
                
                Trace.WriteLine("endSend");
                Thread.Sleep(DELAY_PACKET_HANDLE);
            }
            
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