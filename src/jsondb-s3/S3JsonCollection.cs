using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Amazon.S3;
using JsonDb.Adapters;

namespace JsonDb.S3
{
    internal class S3JsonCollection<T> : InMemoryJsonCollection<T>
    {
        private readonly AmazonS3Client client;
        private readonly string bucketName;
        private readonly string objectKey;
        private readonly IJsonCollectionAdapter collectionAdapter;
        private readonly JsonSerializerOptions serializerOptions;

        public S3JsonCollection( AmazonS3Client s3Client
            , string s3BucketName
            , string s3ObjectKey
            , IJsonCollectionAdapter jsonCollectionAdapter
            , JsonSerializerOptions jsonSerializerOptions )
        {
            client = s3Client;
            bucketName = s3BucketName;
            objectKey = s3ObjectKey;
            collectionAdapter = jsonCollectionAdapter;
            serializerOptions = jsonSerializerOptions;
        }

        protected override async Task WriteAsync( IEnumerable<T> items )
        {
            var json = JsonSerializer.Serialize( items, serializerOptions );
            var utf8Buffer = Encoding.UTF8.GetBytes( json );

            // write through adapter
            if ( collectionAdapter != null )
            {
                utf8Buffer = await collectionAdapter.WriteAsync( utf8Buffer );
            }

            using ( var stream = new MemoryStream( utf8Buffer ) )
            {
                await client.PutObjectAsync( new Amazon.S3.Model.PutObjectRequest
                {
                    BucketName = bucketName,
                    Key = objectKey,
                    InputStream = stream,
                    ContentType = "application/json"
                } );
            }
        }
    }
}
