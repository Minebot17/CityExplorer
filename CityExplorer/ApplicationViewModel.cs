using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using CityExplorer.Annotations;

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

        public ApplicationViewModel()
        {
            Communities = new ObservableCollection<Community>()
            {
                new () { CommunityType = "поселок", FederationSubject = "Московская область", Population = 1000, Title = "Какой-то поселок", FoundationYear = 1920},
                new () { CommunityType = "город", FederationSubject = "Волгоградская область", Population = 2000, Title = "Какой-то город", FoundationYear = 1990},
                new () { CommunityType = "деревня", FederationSubject = "Ростовская область", Population = 3000, Title = "Какая-то деревня", FoundationYear = 1950}
            };

            FederationSubjects = new ObservableCollection<string>() { "Волгоградская область", "Московская область", "Ростовская область" };
            CommunityTypes = new ObservableCollection<string>() { "город", "поселок", "деревня" };
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}