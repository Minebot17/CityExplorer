using System;
using System.IO;
using System.IO.Pipes;
using System.Runtime.InteropServices;
using System.Threading;

namespace CityExplorerServer
{
    internal class Program
    {
        private const int SAVE_PERIOD = 30000;
        
        public static ServerConfig ServerConfig;
        public static Random Random;
        private static int maxClients;
        private static ServerManager serverManager;
        
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
            serverManager.SerializeAllData();
            Environment.Exit(-1);
            return true;
        }
        
        public static void Main(string[] args)
        {
            // Save data befor closing
            _handler += Handler;
            SetConsoleCtrlHandler(_handler, true);
            
            // Timer call save every period
            Timer timer = new Timer(obj => { serverManager.SerializeAllData(); }, null, SAVE_PERIOD, SAVE_PERIOD);

            Random = new Random();
            ServerConfig = new ServerConfig();
            serverManager = new ServerManager(new FileDataSerializer(AppDomain.CurrentDomain.BaseDirectory + ServerConfig["saveFileName"]));

            string serverType = ServerConfig["serverType"];
            maxClients = int.Parse(ServerConfig["maxClients"]);

            if (serverType.Equals("socket"))
            {
                
            }
            else
            {
                int i;
                Thread[] servers = new Thread[maxClients];
                Console.WriteLine("Named pipe server");
                Console.WriteLine("Waiting for client connect...\n");
                
                for (i = 0; i < maxClients; i++)
                {
                    servers[i] = new Thread(ServerThread);
                    servers[i].Start();
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
        
        private static void ServerThread(object data)
        {
            NamedPipeServerStream pipeServer = new NamedPipeServerStream("cityExplorerPipe", PipeDirection.InOut, maxClients);
            int threadId = Thread.CurrentThread.ManagedThreadId;

            while (true)
            {
                pipeServer.WaitForConnection();
                Console.WriteLine("Client connected on thread[{0}].", threadId);

                try
                {
                    StreamString ss = new StreamString(pipeServer);
                    ss.WriteString("I am the one true server!");
                }
                catch (IOException e)
                {
                    Console.WriteLine("ERROR: {0}", e.Message);
                }
                
                pipeServer.Disconnect();
            }

            pipeServer.Close();
        }
        
        
    }
}