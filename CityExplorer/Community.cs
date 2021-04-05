using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using CityExplorer.Annotations;

namespace CityExplorer
{
    public class Community : INotifyPropertyChanged
    {
        private string federationSubject = "";
        private string communityType = "";
        private string title = "";
        private int population;
        private int foundationYear;

        public string FederationSubject
        {
            get => federationSubject;
            set
            {
                federationSubject = value;
                OnPropertyChanged(nameof(FederationSubject));
            }
        }
        
        public string CommunityType
        {
            get => communityType;
            set
            {
                communityType = value;
                OnPropertyChanged(nameof(CommunityType));
            }
        }
        
        public string Title
        {
            get => title;
            set
            {
                title = value;
                OnPropertyChanged(nameof(Title));
            }
        }
        
        public int Population
        {
            get => population;
            set
            {
                population = value;
                OnPropertyChanged(nameof(Population));
            }
        }
        
        public int FoundationYear
        {
            get => foundationYear;
            set
            {
                foundationYear = value;
                OnPropertyChanged(nameof(FoundationYear));
            }
        }

        public override string ToString()
        {
            return Title;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        
        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}