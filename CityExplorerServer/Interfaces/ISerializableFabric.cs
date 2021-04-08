namespace CityExplorerServer
{
    public interface ISerializableFabric<T> where T : IStringSerializable
    {
        T Create();
    }
}