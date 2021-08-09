using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace JsonDb
{
    public interface IJsonCollection<T> : IEnumerable<T>
    {
        void Add( T item );
        void Remove( Func<T, bool> predicate );

        Task WriteAsync();
    }
}
