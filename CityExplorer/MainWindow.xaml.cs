using System;
using System.IO.Pipes;
using System.Security.Principal;
using CityExplorerServer;

namespace CityExplorer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new ApplicationViewModel();
            
            NamedPipeClientStream pipeClient = 
                new NamedPipeClientStream(".", "cityExplorerPipe", PipeDirection.InOut, 
                    PipeOptions.None, TokenImpersonationLevel.Impersonation);

            try
            {
                pipeClient.Connect(1000);
                StreamString ss = new StreamString(pipeClient);

                if (ss.ReadString() == "I am the one true server!")
                {
                    ss.WriteString("c:\\textfile.txt");
                    TestTextBox.Text = ss.ReadString();
                }
                else
                    Console.WriteLine("Server could not be verified.");
            }
            catch (TimeoutException){}
            
            pipeClient.Close();
        }
    }
}