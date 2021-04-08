using System.IO;

namespace CityExplorerServer
{
    public interface IServerOperation<T>
    {
        void Execute(T args, Stream stream);
    }
}