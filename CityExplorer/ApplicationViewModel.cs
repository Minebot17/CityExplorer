﻿using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO.Pipes;
using System.Runtime.CompilerServices;
using System.Security.Principal;
using System.Threading;
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
        public ObservableCollection<Community> Communities { get; set; }

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

        private RelayCommand? addCommunityCommand;
        public RelayCommand AddCommunityCommand => addCommunityCommand ??= new RelayCommand(args =>
        {
            NetworkManager.SendPacketToServer("addCommunityRequest", null);
        });
        
        private RelayCommand? removeCommunityCommand;
        public RelayCommand RemoveCommunityCommand => removeCommunityCommand ??= new RelayCommand(args =>
        {
            long idToRemove = (long) args;
            NetworkManager.SendPacketToServer("removeCommunityRequest", idToRemove);
        });
        
        private RelayCommand? editedCommunityCommand; // Отправляется при любом изменении данных
        public RelayCommand EditedCommunityCommand => editedCommunityCommand ??= new RelayCommand(args =>
        {
            Community selectedCommunity = (Community) args;
            NetworkManager.SendPacketToServer("editCommunityRequest", selectedCommunity);
        });

        public ApplicationViewModel()
        {
            Communities = new ObservableCollection<Community>()
            {
                new (this) { CommunityType = "поселок", FederationSubject = "Московская область", Population = 1000, Title = "Какой-то поселок", FoundationYear = 1920},
                new (this) { CommunityType = "город", FederationSubject = "Волгоградская область", Population = 2000, Title = "Какой-то город", FoundationYear = 1990},
                new (this) { CommunityType = "деревня", FederationSubject = "Ростовская область", Population = 3000, Title = "Какая-то деревня", FoundationYear = 1950}
            };

            FederationSubjects = new ObservableCollection<string>() { "Волгоградская область", "Московская область", "Ростовская область" };
            CommunityTypes = new ObservableCollection<string>() { "город", "поселок", "деревня" };

            Thread clientThread = new Thread(CommunityClient.Start);
            clientThread.Start(this);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}