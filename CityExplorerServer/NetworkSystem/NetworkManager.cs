using System.Collections.Generic;
using System.IO;

namespace CityExplorerServer.NetworkSystem
{
    public static class NetworkManager
    {
        private static Dictionary<string, IPacket> pakets = new Dictionary<string, IPacket>();
        private static Dictionary<string, IPacketHandler> handlers = new Dictionary<string, IPacketHandler>();
        private static Dictionary<string, object> paketsArgs = new Dictionary<string, object>();
        private static Dictionary<string, object> handlersArgs = new Dictionary<string, object>();

        private static bool isServer;
        private static INetworkThread[] serverThreads;
        private static INetworkThread clientThread;

        public static void InitializeServer(INetworkThread[] serverThreads)
        {
            isServer = true;
            NetworkManager.serverThreads = serverThreads;
        }
        
        public static void InitializeClient(INetworkThread clientThread)
        {
            isServer = false;
            NetworkManager.clientThread = clientThread;
        }

        public static void RegisterPacket(IPacket packet)
        {
            pakets[packet.GetPacketName()] = packet;
        }
        
        public static void RegisterHandler(IPacketHandler handler)
        {
            handlers[handler.GetHandledPacketName()] = handler;
        }

        public static void BindArgsToPacket(string paketName, object args)
        {
            paketsArgs[paketName] = args;
        }
        
        public static void BindArgsToHandler(string paketName, object args)
        {
            handlersArgs[paketName] = args;
        }
        
        public static void BindArgsToAllRegisteredPackets(object args)
        {
            foreach (KeyValuePair<string, IPacket> pair in pakets)
                paketsArgs[pair.Key] = args;
        }
        
        public static void BindArgsToAllRegisteredHandlers(object args)
        {
            foreach (KeyValuePair<string, IPacketHandler> pair in handlers)
                handlersArgs[pair.Key] = args;
        }

        public static void RecievePacketFromThread(string packetName, PacketStream stream)
        {
            handlers[packetName].Handle(handlersArgs.ContainsKey(packetName) ? handlersArgs[packetName] : null, stream);
        }
        
        public static void SendPacketToOtherSide(string packetName, object args, PacketStream stream)
        {
            pakets[packetName].Write(paketsArgs.ContainsKey(packetName) ? paketsArgs[packetName] : null, args, stream);
        }

        public static void SendPacketToClient(string packetName, object args, INetworkThread clientFor)
        {
            if (clientFor == null)
                throw new IOException("Client is null");
            
            if (!isServer && clientThread != null)
                throw new IOException("You cant send packets from client to other client");

            if (!clientFor.IsConnected())
                throw new IOException("Client what you try to send packet is disconnected");

            clientFor.AddToSendQueue(packetName, args);
        }

        public static void SendPacketToAllClients(string packetName, object args)
        {
            if (!isServer && clientThread != null)
                throw new IOException("You cant send packets from client to other client");

            if (serverThreads == null)
                throw new IOException("You didnt initialize NetworkManager");

            lock (serverThreads)
            {
                foreach (INetworkThread serverThread in serverThreads)
                    if (serverThread.IsConnected())
                        serverThread.AddToSendQueue(packetName, args);
            }
        }

        public static void SendPacketToServer(string packetName, object args)
        {
            if (isServer)
                throw new IOException("You cant send packets from server to self");
            
            if (clientThread == null)
                throw new IOException("You didnt initialize NetworkManager");

            if (!clientThread.IsConnected())
                throw new IOException("You dont connected with server");
            
            clientThread.AddToSendQueue(packetName, args);
        }
    }
}