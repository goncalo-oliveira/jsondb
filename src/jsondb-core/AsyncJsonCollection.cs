using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace JsonDb
{
    internal class AsyncJsonCollection<T> : IAsyncJsonCollection<T>
    {
        private readonly Lazy<Task<IJsonCollection<T>>> lazy;

        public AsyncJsonCollection( IJsonDb jsonDb, string collectionName )
        {
            lazy = new Lazy<Task<IJsonCollection<T>>>( () => Task.Factory.StartNew( 
                () => jsonDb.GetCollectionAsync<T>( collectionName )
            ).Unwrap() );
        }

        public Task<IJsonCollection<T>> GetCollectionAsync() => lazy.Value;

        public void Add( T item )
        {
            GetCollectionAsync()
                .GetAwaiter()
                .GetResult()
                .Add( item );
        }

        public void Remove( Func<T, bool> predicate )
        {
            GetCollectionAsync()
                .GetAwaiter()
                .GetResult()
                .Remove( predicate );
        }

        public Task WriteAsync()
        {
            return GetCollectionAsync()
                .ContinueWith( previousTask => previousTask.Result.WriteAsync() );
        }

        public IEnumerator<T> GetEnumerator()
        {
            return GetCollectionAsync()
                .GetAwaiter()
                .GetResult()
                .GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetCollectionAsync()
                .GetAwaiter()
                .GetResult()
                .GetEnumerator();
        }
    }
}
