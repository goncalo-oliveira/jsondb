using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace JsonDb.Local
{
    internal class LocalJsonCollection<T> : InMemoryJsonCollection<T>
    {
        private readonly string filePath;
        private readonly JsonSerializerOptions serializerOptions;

        public LocalJsonCollection( string jsonFilePath, JsonSerializerOptions jsonSerializerOptions )
        {
            filePath = jsonFilePath;
            serializerOptions = jsonSerializerOptions;
        }

        protected override Task WriteAsync( IEnumerable<T> items )
        {
            var json = JsonSerializer.Serialize( items, serializerOptions );

            return File.WriteAllTextAsync( filePath, json, Encoding.UTF8 );
        }
    }
}
