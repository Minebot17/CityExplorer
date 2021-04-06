using System;
using System.IO;
using System.IO.Pipes;
using System.Threading;

namespace CityExplorerServer
{
    internal class Program
    {
        public static ServerConfig ServerConfig;
        public static Random Random;
        private static int maxClients;
        private static ServerManager serverManager;
        
        public static long RandomLong()
        {
            byte[] bytes = new byte[8];
            Random.NextBytes(bytes);
            return BitConverter.ToInt64(bytes, 0);
        }
        
        public static void Main(string[] args)
        {
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
                    Console.WriteLine(ss.ReadString());
                    ss.WriteString("ServerConnected");
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