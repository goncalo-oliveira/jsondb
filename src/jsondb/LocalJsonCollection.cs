using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using JsonDb.Adapters;

namespace JsonDb.Local
{
    internal class LocalJsonCollection<T> : InMemoryJsonCollection<T>
    {
        private readonly Encoding encoding = new UTF8Encoding( false );
        private readonly string filePath;
        private readonly IJsonCollectionAdapter collectionAdapter;
        private readonly JsonSerializerOptions serializerOptions;

        public LocalJsonCollection( string jsonFilePath
            , IJsonCollectionAdapter jsonCollectionAdapter
            , JsonSerializerOptions jsonSerializerOptions )
        {
            filePath = jsonFilePath;
            collectionAdapter = jsonCollectionAdapter;
            serializerOptions = jsonSerializerOptions;
        }

        protected override async Task WriteAsync( IEnumerable<T> items )
        {
            var json = JsonSerializer.Serialize( items, serializerOptions );
            var utf8Buffer = encoding.GetBytes( json );

            // write through adapter
            if ( collectionAdapter != null )
            {
                utf8Buffer = await collectionAdapter.WriteAsync( utf8Buffer );
            }

            await File.WriteAllBytesAsync( filePath, utf8Buffer );
        }
    }
}
