using System;
using System.Collections.Generic;
using System.IO.Pipes;
using System.Security.Principal;
using CityExplorer.Handlers;
using CityExplorer.Packets;
using CityExplorerServer;
using CityExplorerServer.NetworkSystem;

namespace CityExplorer
{
    public static class CommunityClient
    {
        private static readonly List<IPacket> packetsToRegister = new List<IPacket>()
        {
            new AddCommunityPacket(),
            new RemoveCommunityPacket(),
            new EditCommunityPacket()
        };
        
        private static readonly List<IPacketHandler> handlersToRegister = new List<IPacketHandler>()
        {
            new AddCommuntiyHandler(),
            new RemoveCommunityHandler(),
            new EditCommunityHandler()
        };
        
        public static void Start(object args)
        {
            ApplicationViewModel viewModel = (ApplicationViewModel) args;
            INetworkThread clientThread = new NetworkThread();
            NetworkManager.InitializeClient(clientThread);
            
            foreach (IPacket packet in packetsToRegister)
                NetworkManager.RegisterPacket(packet);
            
            foreach (IPacketHandler handler in handlersToRegister)
                NetworkManager.RegisterHandler(handler);
            
            NetworkManager.BindArgsToAllRegisteredHandlers(viewModel);

            NamedPipeClientStream pipeClient = 
                new NamedPipeClientStream(".", "cityExplorerPipe", PipeDirection.InOut, 
                    PipeOptions.None, TokenImpersonationLevel.Impersonation);
            
            clientThread.SetStream(pipeClient);

            try
            {
                pipeClient.Connect(2000);
                clientThread.OnConnected();
                StreamString ss = new StreamString(pipeClient);

                if (ss.ReadString() == "I am the one true server!")
                {
                    while (true)
                        clientThread.HandleStream();
                }
                else
                    Console.WriteLine("Server could not be verified.");
            }
            catch (TimeoutException){}
            
            clientThread.OnDisconnected();
            pipeClient.Close();
        }
    }
}