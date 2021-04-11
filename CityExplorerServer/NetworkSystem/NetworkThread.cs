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

        public bool HandleStreamRead()
        {
            if (!stream.CanRead)
            {
                Thread.Sleep(100);
                return true;
            }
            
            Trace.WriteLine("startObserve");
            int r = stream.ReadByte();
            Trace.WriteLine("endObserve");
                
            //if (r)
            //{
                Trace.WriteLine("startRecieve");
                string packetName = streamString.ReadString();
                NetworkManager.RecievePacketFromThread(packetName, packetStream, this);
                Trace.WriteLine("endRecieve");
            //}
            return true;
        }

        public bool HandleStreamWrite()
        {
            if (sendQueue.Count == 0 || !stream.CanWrite)
            {
                Thread.Sleep(33);
                return true;
            }
            
                Trace.WriteLine("startSend");
                try
                {
                    stream.WriteByte(1);
                    (string, object) paketUnit = sendQueue.Dequeue();
                    packetStream.Write(paketUnit.Item1);
                    NetworkManager.SendPacketToOtherSide(paketUnit.Item1, paketUnit.Item2, packetStream);
                    packetStream.Flush();
                    ((PipeStream)stream).WaitForPipeDrain();
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