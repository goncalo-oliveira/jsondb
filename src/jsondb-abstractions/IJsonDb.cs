using System;
using System.Threading.Tasks;

namespace JsonDb
{
    public interface IJsonDb
    {
        Task<IJsonCollection<T>> GetCollectionAsync<T>( string name );
    }
}
