using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
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

        public void HandleStreamRead()
        {
            if (!stream.CanRead)
            {
                Thread.Sleep(DELAY_PACKET_HANDLE);
                return;
            }
            
            Trace.WriteLine("startObserve");
            string packetName = streamString.ReadString();
            NetworkManager.RecievePacketFromThread(packetName, packetStream, this);
            Trace.WriteLine("received");
        }

        public bool HandleStreamWrite()
        {
            if (sendQueue.Count == 0 || !stream.CanWrite)
            {
                Thread.Sleep(DELAY_PACKET_HANDLE);
                return true;
            }
            
            Trace.WriteLine("startSend");
            try
            {
                (string, object) paketUnit = sendQueue.Dequeue();
                packetStream.Write(paketUnit.Item1);
                NetworkManager.SendPacketToOtherSide(paketUnit.Item1, paketUnit.Item2, packetStream);
                packetStream.Flush();
                
                if (stream is PipeStream pipe)
                    pipe.WaitForPipeDrain();
            }
            catch (IOException e)
            {
                Trace.WriteLine(e);
                return false;
            }
            
            Trace.WriteLine("endSend");
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