using System.Collections.Generic;

namespace CityExplorerServer
{
    public interface IDataSerializer
    {
        void Save<T>(List<T> data) where T : IStringSerializable;
        List<T> Load<T>(ISerializableFabric<T> serializableFabric) where T : IStringSerializable;
    }
}