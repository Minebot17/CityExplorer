using System.Collections.Generic;

namespace CityExplorerServer
{
    public interface IStringSerializable
    {
        List<string> Serialize();
        void Deserialize(List<string> lines);
        int GetLinesSize();
    }
}