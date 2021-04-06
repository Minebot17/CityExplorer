using System.Collections.Generic;

namespace CityExplorerServer
{
    public interface IDataSerializer
    {
        void Save(List<IStringSerializable> data);
        List<IStringSerializable> Load(ISerializableFabric serializableFabric);
    }
}