using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO.Pipes;
using System.Runtime.CompilerServices;
using System.Security.Principal;
using System.Threading;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using CityExplorer.Annotations;
using CityExplorerServer;
using CityExplorerServer.NetworkSystem;

namespace CityExplorer
{
    public class ApplicationViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<string> FederationSubjects { get; set; }
        public ObservableCollection<string> CommunityTypes { get; set; }
        public ObservableCollection<string> ConnectionTypes { get; set; }

        private readonly object communitiesLock = new object();
        private ObservableCollection<Community> communities;

        public ObservableCollection<Community> Communities
        {
            get => communities;
            private set
            {
                communities = value;
                BindingOperations.EnableCollectionSynchronization(communities, communitiesLock);
            }
        }

        private Community selectedCommunity;
        public Community SelectedCommunity
        {
            get => selectedCommunity;
            set
            {
                selectedCommunity = value;
                OnPropertyChanged(nameof(SelectedCommunity));
            }
        }

        private Visibility showPort = Visibility.Hidden;

        public Visibility ShowPort
        {
            get => showPort;
            set
            {
                showPort = value;
                OnPropertyChanged(nameof(ShowPort));
            }
        }

        private string selectedConnectionType = "Pipe";
        public string SelectedConnectionType {
            get => selectedConnectionType;
            set
            {
                selectedConnectionType = value;
                OnPropertyChanged(nameof(SelectedConnectionType));
                ShowPort = value.Equals("TCP") ? Visibility.Visible : Visibility.Hidden;
            }
        }
        
        private string tcpIp = "127.0.0.1";
        public string TcpIp
        {
            get => tcpIp;
            set
            {
                tcpIp = value;
                OnPropertyChanged(nameof(TcpIp));
            }
        }

        private int tcpPort = 8888;
        public int TcpPort
        {
            get => tcpPort;
            set
            {
                tcpPort = value;
                OnPropertyChanged(nameof(TcpPort));
            }
        }

        private RelayCommand addCommunityCommand;
        public RelayCommand AddCommunityCommand => addCommunityCommand ?? (addCommunityCommand = new RelayCommand(args =>
        {
            NetworkManager.SendPacketToServer("addCommunityRequest", null);
        }));
        
        private RelayCommand removeCommunityCommand;
        public RelayCommand RemoveCommunityCommand => removeCommunityCommand ?? (removeCommunityCommand = new RelayCommand(args =>
        {
            long idToRemove = (long) args;
            NetworkManager.SendPacketToServer("removeCommunityRequest", idToRemove);
        }));
        
        private RelayCommand editedCommunityCommand; // Отправляется при любом изменении данных
        public RelayCommand EditedCommunityCommand => editedCommunityCommand ?? (editedCommunityCommand= new RelayCommand(args =>
        {
            Community selectedCommunity = (Community) args;
            NetworkManager.SendPacketToServer("editCommunityRequest", selectedCommunity);
        }));
        
        private RelayCommand reconnectCommand;
        public RelayCommand ReconnectCommand => reconnectCommand ?? (reconnectCommand= new RelayCommand(args =>
        {
            clientThread.Abort();
            ConnectToServer();
        }));

        private Thread clientThread;

        public ApplicationViewModel()
        {
            Communities = new ObservableCollection<Community>();
            FederationSubjects = new ObservableCollection<string>() { "Волгоградская область", "Московская область", "Ростовская область" };
            CommunityTypes = new ObservableCollection<string>() { "город", "поселок", "деревня" };
            ConnectionTypes = new ObservableCollection<string>() { "Pipe", "TCP" };
            
            ConnectToServer();
        }

        private void ConnectToServer()
        {
            clientThread = new Thread(CommunityClient.Start);
            clientThread.Start((this, selectedConnectionType.Equals("Pipe") ? 0 : tcpPort, tcpIp));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            if (propertyName.Equals(nameof(TcpPort)) || propertyName.Equals(nameof(TcpIp)) || propertyName.Equals(nameof(SelectedConnectionType)))
                ReconnectCommand.Execute(null);
        }
    }
}