using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JsonDb
{
    public class InMemoryJsonCollection<T> : IJsonCollection<T>
    {
        private readonly List<T> collection;

        public InMemoryJsonCollection()
        {
            collection = new List<T>();
        }

        public InMemoryJsonCollection( int capacity )
        {
            collection = new List<T>( capacity );
        }

        public InMemoryJsonCollection( IEnumerable<T> items )
        {
            collection = new List<T>( items );
        }

        public void Add( T item )
        {
            collection.Add( item );
        }

        public void Remove( Func<T, bool> predicate )
        {
            var remove = collection.Where( predicate )
                .ToArray();

            foreach ( var item in remove )
            {
                collection.Remove( item );
            }
        }

        public Task WriteAsync()
        {
            return WriteAsync( collection );
        }

        protected virtual Task WriteAsync( IEnumerable<T> items )
        {
            throw new NotSupportedException();
        }

        public IEnumerator<T> GetEnumerator()
            => collection.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => collection.GetEnumerator();
    }
}
