using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Amazon.S3;

namespace JsonDb.S3
{
    internal class S3JsonDb : IJsonDb
    {
        private readonly AmazonS3Client client;
        private readonly string bucketName;
        private readonly string dbPath;
        private readonly JsonSerializerOptions serializerOptions;

        public S3JsonDb( AmazonS3Client s3Client, string s3BucketName, string s3Prefix, JsonSerializerOptions jsonSerializerOptions = null )
        {
            client = s3Client;
            bucketName = s3BucketName;
            dbPath = s3Prefix.TrimEnd( '/' );
            serializerOptions = jsonSerializerOptions;
        }

        public async Task<IJsonCollection<T>> GetCollectionAsync<T>( string name )
        {
            var key = $"{dbPath}/{name}.json";

            var collection = new S3JsonCollection<T>( client, bucketName, key, serializerOptions );

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

            var getResponse = await client.GetObjectAsync( bucketName, key );

            using ( getResponse.ResponseStream )
            {
                var items = await JsonSerializer.DeserializeAsync<IEnumerable<T>>( getResponse.ResponseStream, serializerOptions );

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
