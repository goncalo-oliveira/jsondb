using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using JsonDb.Adapters;

namespace JsonDb.Local
{
    internal class LocalJsonDb : IJsonDb
    {
        private readonly Encoding encoding = new UTF8Encoding( false );
        private readonly string dbPath;
        private readonly IJsonCollectionAdapter collectionAdapter;
        private readonly JsonSerializerOptions serializerOptions;

        public LocalJsonDb( string path, IJsonCollectionAdapter jsonCollectionAdapter, JsonSerializerOptions jsonSerializerOptions )
        {
            if ( Path.IsPathRooted( path ) )
            {
                dbPath = path;
            }
            else
            {
                dbPath = Path.GetFullPath( path, Directory.GetCurrentDirectory() );
            }

            if ( !Directory.Exists( dbPath ) )
            {
                throw new DirectoryNotFoundException();
            }

            collectionAdapter = jsonCollectionAdapter;
            serializerOptions = jsonSerializerOptions;
        }

        public async Task<IJsonCollection<T>> GetCollectionAsync<T>( string name )
        {
            var filepath = Path.Combine( dbPath, $"{name}.json" );

            if ( !File.Exists( filepath ) )
            {
                File.CreateText( filepath )
                    .Close();
            }

            var utf8Buffer = await File.ReadAllBytesAsync( filepath );

            var collection = new LocalJsonCollection<T>( filepath, collectionAdapter, serializerOptions );

            if ( !( utf8Buffer?.Any() == true ) )
            {
                return ( collection );
            }

            // read through adapter
            if ( collectionAdapter != null )
            {
                utf8Buffer = await collectionAdapter.ReadAsync( utf8Buffer );
            }

            var json = encoding.GetString( utf8Buffer );

            var items = JsonSerializer.Deserialize<IEnumerable<T>>( json, serializerOptions );

            if ( items?.Any() == true )
            {
                foreach ( var item in items )
                {
                    collection.Add( item );
                }
            }

            return ( collection );
        }
    }
}
