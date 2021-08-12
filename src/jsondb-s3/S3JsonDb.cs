using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Amazon.S3;
using JsonDb.Adapters;

namespace JsonDb.S3
{
    internal class S3JsonDb : IJsonDb
    {
        private readonly AmazonS3Client client;
        private readonly string bucketName;
        private readonly string dbPath;
        private readonly IJsonCollectionAdapter collectionAdapter;
        private readonly JsonSerializerOptions serializerOptions;

        public S3JsonDb( AmazonS3Client s3Client
            , string s3BucketName
            , string s3Prefix
            , IJsonCollectionAdapter jsonCollectionAdapter
            , JsonSerializerOptions jsonSerializerOptions )
        {
            client = s3Client;
            bucketName = s3BucketName;
            dbPath = s3Prefix.TrimEnd( '/' );
            collectionAdapter = jsonCollectionAdapter;
            serializerOptions = jsonSerializerOptions;
        }

        public async Task<IJsonCollection<T>> GetCollectionAsync<T>( string name )
        {
            var key = $"{dbPath}/{name}.json";

            var collection = new S3JsonCollection<T>( client, bucketName, key, collectionAdapter, serializerOptions );

            var listResponse = await client.ListObjectsV2Async( new Amazon.S3.Model.ListObjectsV2Request
            {
                BucketName = bucketName,
                Prefix = key,
                MaxKeys = 1
            });

            var existingObject = listResponse.S3Objects?.SingleOrDefault();

            if ( existingObject == null )
            {
                // collection doesn't exist yet
                return ( collection );
            }

            var response = await client.GetObjectAsync( bucketName, key );

            byte[] utf8Buffer;
            using ( response.ResponseStream )
            {
                using ( var ms = new MemoryStream() )
                {
                    await response.ResponseStream.CopyToAsync( ms );

                    utf8Buffer = ms.ToArray();

                    if ( !( utf8Buffer?.Any() == true ) )
                    {
                        return ( collection );
                    }

                    // read through adapter
                    if ( collectionAdapter != null )
                    {
                        utf8Buffer = await collectionAdapter.ReadAsync( utf8Buffer );
                    }
                }

                var items = JsonSerializer.Deserialize<IEnumerable<T>>( utf8Buffer, serializerOptions );

                if ( items?.Any() == true )
                {
                    foreach ( var item in items )
                    {
                        collection.Add( item );
                    }
                }
            }

            return ( collection );
        }
    }
}
