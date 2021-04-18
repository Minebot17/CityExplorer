using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using CityExplorerServer.Handlers;
using CityExplorerServer.NetworkSystem;
using CityExplorerServer.Packets;

namespace CityExplorerServer
{
    internal class Program
    {
        private const int SAVE_PERIOD = 30000;
        
        public static Random Random;
        private static ServerConfig ServerConfig;
        private static int maxClients;
        private static CommunityManager communityManager;
        private static INetworkThread[] serverThreads;
        
        private static readonly List<IPacket> packetsToRegister = new List<IPacket>()
        {
            new AddCommunityPacket(),
            new RemoveCommunityPacket(),
            new EditCommunityPacket(),
            new GetAllDataPacket()
        };
        
        private static readonly List<IPacketHandler> handlersToRegister = new List<IPacketHandler>()
        {
            new AddCommunityHandler(),
            new RemoveCommunityHandler(),
            new EditCommunityHandler(),
            new GetAllDataHandler()
        };
        
        
        [DllImport("Kernel32")]
        private static extern bool SetConsoleCtrlHandler(EventHandler handler, bool add);

        private delegate bool EventHandler(CtrlType sig);
        static EventHandler _handler;

        enum CtrlType
        {
            CTRL_C_EVENT = 0,
            CTRL_BREAK_EVENT = 1,
            CTRL_CLOSE_EVENT = 2,
            CTRL_LOGOFF_EVENT = 5,
            CTRL_SHUTDOWN_EVENT = 6
        }

        private static bool Handler(CtrlType sig)
        {
            communityManager.SerializeAllData();
            Environment.Exit(-1);
            return true;
        }
        
        public static void Main(string[] args)
        {
            // Save data befor closing
            _handler += Handler;
            SetConsoleCtrlHandler(_handler, true);
            
            // Timer call save every period
            Timer timer = new Timer(obj => { communityManager.SerializeAllData(); }, null, SAVE_PERIOD, SAVE_PERIOD);

            Random = new Random();
            ServerConfig = new ServerConfig();
            communityManager = new CommunityManager(new FileDataSerializer(AppDomain.CurrentDomain.BaseDirectory + ServerConfig["saveFileName"]));

            string serverType = ServerConfig["serverType"];
            maxClients = int.Parse(ServerConfig["maxClients"]);
            serverThreads = new INetworkThread[maxClients];
            for (int i = 0; i < serverThreads.Length; i++)
                serverThreads[i] = new NetworkThread();
            
            // Setup NetworkManager
            NetworkManager.InitializeServer(serverThreads);
            
            foreach (IPacket packet in packetsToRegister)
                NetworkManager.RegisterPacket(packet);
            
            foreach (IPacketHandler handler in handlersToRegister)
                NetworkManager.RegisterHandler(handler);
            
            NetworkManager.BindArgsToAllRegisteredHandlers(communityManager);

            if (serverType.Equals("socket"))
            {
                IPAddress localAddr = IPAddress.Parse("127.0.0.1");
                TcpListener server = new TcpListener(localAddr, int.Parse(ServerConfig["tcpPort"]));
                server.Start();

                while (true)
                {
                    TcpClient client = server.AcceptTcpClient();
                    INetworkThread firstFree = serverThreads.FirstOrDefault(t => !t.IsConnected());
                
                    if (firstFree != null)
                        new Thread(ServerSocketThread).Start((client.GetStream(), firstFree));
                }
            }
            else
            {
                int i;
                Thread[] servers = new Thread[maxClients];
                Console.WriteLine("Named pipe server");
                Console.WriteLine("Waiting for client connect...\n");
                
                for (i = 0; i < maxClients; i++)
                {
                    servers[i] = new Thread(ServerPipeThread);
                    servers[i].Start(serverThreads[i]);
                }
                
                Thread.Sleep(250);
                while (i > 0)
                {
                    for (int j = 0; j < maxClients; j++)
                    {
                        if (servers[j] != null)
                        {
                            if (servers[j].Join(250))
                            {
                                Console.WriteLine("Server thread[{0}] finished.", servers[j].ManagedThreadId);
                                servers[j] = null;
                                i--;
                            }
                        }
                    }
                }
                
                Console.WriteLine("\nServer threads exhausted, exiting.");
            }
        }
        
        public static long RandomLong()
        {
            byte[] bytes = new byte[8];
            Random.NextBytes(bytes);
            return BitConverter.ToInt64(bytes, 0);
        }

        private static void ServerSocketThread(object data)
        {
            (NetworkStream, INetworkThread) args = ((NetworkStream, INetworkThread)) data;
            INetworkThread networkThread = args.Item2;
            NetworkStream stream = args.Item1;
            
            networkThread.SetStream(stream);
            networkThread.OnConnected();
            
            try
            {
                Thread readThread = new Thread(() =>
                {
                    while (true)
                        networkThread.HandleStreamRead();
                });
                readThread.Start();
                    
                while (true)
                {
                    bool result = networkThread.HandleStreamWrite();
                    if (!result)
                    {
                        readThread.Abort();
                        break;
                    }
                }
            }
            catch (IOException e)
            {
                Trace.WriteLine("ERROR: {0}", e.Message);
            }
            
            networkThread.OnDisconnected();
            stream.Close();
        }
        
        private static void ServerPipeThread(object data)
        {
            NamedPipeServerStream pipeServer = new NamedPipeServerStream("cityExplorerPipe", PipeDirection.InOut, maxClients, PipeTransmissionMode.Byte, PipeOptions.Asynchronous);
            int threadId = Thread.CurrentThread.ManagedThreadId;
            INetworkThread networkThread = (INetworkThread) data;
            networkThread.SetStream(pipeServer);

            while (true)
            {
                pipeServer.WaitForConnection();
                Console.WriteLine("Client connected on thread[{0}].", threadId);
                networkThread.OnConnected();

                try
                {
                    Thread readThread = new Thread(() =>
                    {
                        while (true)
                            networkThread.HandleStreamRead();
                    });
                    readThread.Start();
                    
                    while (true)
                    {
                        bool result = networkThread.HandleStreamWrite();
                        if (!result)
                        {
                            readThread.Abort();
                            break;
                        }
                    }
                }
                catch (IOException e)
                {
                    Trace.WriteLine("ERROR: {0}", e.Message);
                }
                
                pipeServer.Disconnect();
                networkThread.OnDisconnected();
                Trace.WriteLine("disconnected");
            }

            pipeServer.Close();
        }
    }
}