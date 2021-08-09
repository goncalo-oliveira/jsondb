using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Amazon.S3;

namespace JsonDb.S3
{
    internal class S3JsonCollection<T> : InMemoryJsonCollection<T>
    {
        private readonly AmazonS3Client client;
        private readonly string bucketName;
        private readonly string objectKey;
        private readonly JsonSerializerOptions serializerOptions;

        public S3JsonCollection( AmazonS3Client s3Client, string s3BucketName, string s3ObjectKey, JsonSerializerOptions jsonSerializerOptions = null )
        {
            client = s3Client;
            bucketName = s3BucketName;
            objectKey = s3ObjectKey;
            serializerOptions = jsonSerializerOptions;
        }

        protected override async Task WriteAsync( IEnumerable<T> items )
        {
            using ( var stream = new MemoryStream() )
            {
                await JsonSerializer.SerializeAsync( stream, items, serializerOptions );

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
