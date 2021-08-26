using System;
using System.Threading.Tasks;

namespace JsonDb
{
    public static class JsonDbAsyncExtensions
    {
        public static IAsyncJsonCollection<T> GetAsyncCollection<T>( this IJsonDb db, string collectionName, bool preload = false )
        {
            var collection = new AsyncJsonCollection<T>( db, collectionName );

            if ( preload )
            {
                Task.Factory.StartNew( () => collection.GetCollectionAsync() );
            }

            return ( collection );
        }
    }
}
