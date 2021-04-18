using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Pipes;
using System.Net.Sockets;
using System.Security.Principal;
using System.Threading;
using System.Windows;
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
            (ApplicationViewModel viewModel, int port) data = ((ApplicationViewModel, int)) args;
            bool isTcp = data.port != 0;
            INetworkThread clientThread = new NetworkThread();
            NetworkManager.InitializeClient(clientThread);
            
            foreach (IPacket packet in packetsToRegister)
                NetworkManager.RegisterPacket(packet);
            
            foreach (IPacketHandler handler in handlersToRegister)
                NetworkManager.RegisterHandler(handler);
            
            NetworkManager.BindArgsToAllRegisteredHandlers(data.viewModel);

            NamedPipeClientStream pipeClient = null;
            TcpClient tcpClient = null;

            if (isTcp)
                tcpClient = new TcpClient();
            else
                pipeClient = new NamedPipeClientStream(".", "cityExplorerPipe", PipeDirection.InOut, 
                    PipeOptions.Asynchronous, TokenImpersonationLevel.Impersonation);

            try
            {
                if (isTcp)
                {
                    tcpClient.Connect("127.0.0.1", 8888);
                    clientThread.SetStream(tcpClient.GetStream());
                }
                else
                {
                    pipeClient.Connect(2000);
                    clientThread.SetStream(pipeClient);
                }


                clientThread.OnConnected();
                NetworkManager.SendPacketToServer("getAllDataRequest", null);

                Thread readThread = new Thread(() =>
                {
                    while (true)
                        clientThread.HandleStreamRead();
                });
                readThread.Start();

                while (true)
                {
                    bool result = clientThread.HandleStreamWrite();
                    if (!result)
                    {
                        readThread.Abort();
                        break;
                    }
                }
            }
            catch (TimeoutException e)
            {
                Trace.WriteLine(e.Message);
                MessageBox.Show("Server not running", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                data.viewModel.Communities.Clear();
                //Application.Current.Shutdown();
            }
            catch (SocketException e)
            {
                Trace.WriteLine(e.Message);
                MessageBox.Show("Server not running", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                data.viewModel.Communities.Clear();
            }
            
            clientThread.OnDisconnected();
            
            if (isTcp)
                tcpClient.Close();
            else
                pipeClient.Close();
        }
    }
}