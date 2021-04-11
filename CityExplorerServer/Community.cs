using System.Collections.Generic;

namespace CityExplorerServer
{
    public class Community : IStringSerializable
    {
        public long Id { get; private set; }
        private string FederationSubject { get; set; } = "Московская область";
        private string CommunityType { get; set; } = "город";
        private string Title { get; set; } = "Москва";
        private int Population { get; set; } = 12000000;
        private int FoundationYear { get; set; } = 1147;

        public Community()
        {
            Id = Program.RandomLong();
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
            Id = long.Parse(lines[0]);
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
    }
}