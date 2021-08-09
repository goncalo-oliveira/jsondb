using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace JsonDb
{
    /// <summary>
    /// Extensions to read directly into a readonly collection
    /// </summary>
    public static class JsonDbCollection
    {
        private static readonly HttpClient httpClient = new HttpClient();
        private static readonly JsonDbOptions options = new JsonDbOptions();

        /// <summary>
        /// Reads JSON from stream into a readonly collection
        /// </summary>
        /// <param name="stream">The stream to read from</param>
        /// <param name="serializerOptions">Optional JSON serializer options</param>
        /// <returns>A readonly JsonDb.IJsonCollection<T> instance</returns>
        public static async Task<IJsonCollection<T>> ReadStreamAsync<T>( Stream stream, JsonSerializerOptions serializerOptions = null )
        {
            var items = await JsonSerializer.DeserializeAsync<IEnumerable<T>>( stream, serializerOptions ?? options.JsonSerializerOptions );

            if ( !( items?.Any() == true ) )
            {
                return ( null );
            }

            return new InMemoryJsonCollection<T>( items );
        }

        /// <summary>
        /// Reads JSON from file into a readonly collection
        /// </summary>
        /// <param name="filepath">The path to the JSON file</param>
        /// <param name="serializerOptions">Optional JSON serializer options</param>
        /// <returns>A readonly JsonDb.IJsonCollection<T> instance</returns>
        public static async Task<IJsonCollection<T>> ReadFileAsync<T>( string filepath, JsonSerializerOptions serializerOptions = null )
        {
            using ( var stream = File.OpenRead( filepath ) )
            {
                return await ReadStreamAsync<T>( stream, serializerOptions );
            }
        }

        /// <summary>
        /// Reads JSON from an uri into a readonly collection
        /// </summary>
        /// <param name="uri">The uri of the JSON file</param>
        /// <param name="serializerOptions">Optional JSON serializer options</param>
        /// <returns>A readonly JsonDb.IJsonCollection<T> instance</returns>
        public static Task<IJsonCollection<T>> ReadUrlAsync<T>( string uri, JsonSerializerOptions serializerOptions = null )
            => ReadUrlAsync<T>( httpClient, uri, serializerOptions );

        /// <summary>
        /// Reads JSON from an uri into a readonly collection
        /// </summary>
        /// <param name="httpClient">The System.Net.Http.HttpClient instance to use to request the uri</param>
        /// <param name="uri">The uri of the JSON file</param>
        /// <param name="serializerOptions">Optional JSON serializer options</param>
        /// <returns>A readonly JsonDb.IJsonCollection<T> instance</returns>
        public static async Task<IJsonCollection<T>> ReadUrlAsync<T>( HttpClient httpClient, string uri, JsonSerializerOptions serializerOptions = null )
        {
            using ( var response = await httpClient.GetAsync( uri, HttpCompletionOption.ResponseHeadersRead ) )
            {
                response.EnsureSuccessStatusCode();

                using ( var stream = await response.Content.ReadAsStreamAsync() )
                {
                    return await ReadStreamAsync<T>( stream, serializerOptions );
                }
            }
        }
    }
}
