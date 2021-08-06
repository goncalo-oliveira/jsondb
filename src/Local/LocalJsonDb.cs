using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace JsonDb.Files
{
    internal class LocalJsonDb : IJsonDb
    {
        private readonly string dbPath;
        private readonly JsonSerializerOptions serializerOptions;

        public LocalJsonDb( string path, JsonSerializerOptions jsonSerializerOptions = null )
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

            var json = await File.ReadAllTextAsync( filepath );

            var collection = new LocalJsonCollection<T>( filepath, serializerOptions );

            if ( string.IsNullOrEmpty( json ) )
            {
                return ( collection );
            }

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
