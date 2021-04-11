using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Pipes;
using System.Security.Principal;
using System.Threading;
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
            new EditCommunityPacket(),
            new GetAllDataPacket()
        };
        
        private static readonly List<IPacketHandler> handlersToRegister = new List<IPacketHandler>()
        {
            new AddCommuntiyHandler(),
            new RemoveCommunityHandler(),
            new EditCommunityHandler(),
            new GetAllDataHandler()
        };
        
        public static void Start(object args)
        {
            Thread.CurrentThread.IsBackground = true;
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
                    NetworkManager.SendPacketToServer("getAllDataRequest", null);

                    while (true)
                        clientThread.HandleStream();
                }
                else
                    Trace.WriteLine("Server could not be verified.");
            }
            catch (TimeoutException e)
            {
                Trace.WriteLine(e.Message);
            }
            
            clientThread.OnDisconnected();
            pipeClient.Close();
        }
    }
}