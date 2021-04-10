using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using CityExplorer.Annotations;
using CityExplorerServer;

namespace CityExplorer
{
    public class Community : INotifyPropertyChanged, IStringSerializable
    {
        private string federationSubject = "";
        private string communityType = "";
        private string title = "";
        private int population;
        private int foundationYear;
        
        public long Id { get; set; }

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
        
        public List<string> Serialize()
        {
            return new List<string>()
            {
                Id.ToString(),
                FederationSubject,
                CommunityType,
                Title,
                Population.ToString(),
                FoundationYear.ToString()
            };
        }

        public void Deserialize(List<string> lines)
        {
            Id = int.Parse(lines[0]);
            FederationSubject = lines[1];
            CommunityType = lines[2];
            Title = lines[3];
            Population = int.Parse(lines[4]);
            FoundationYear = int.Parse(lines[5]);
        }

        public int GetLinesSize()
        {
            return 6;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        
        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}