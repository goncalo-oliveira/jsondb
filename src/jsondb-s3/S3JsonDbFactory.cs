using System;
using Amazon.Runtime;
using Amazon.S3;
using Microsoft.Extensions.Options;

namespace JsonDb.S3
{
    internal class S3JsonDbFactory : IJsonDbFactory
    {
        private readonly S3JsonDbOptions options;
        private readonly AWSCredentials credentials;
        private readonly AmazonS3Config s3Config;

        public S3JsonDbFactory( IOptions<S3JsonDbOptions> optionsAccessor )
        {
            options = optionsAccessor.Value;
            credentials = new BasicAWSCredentials( options.AccessKey, options.SecretKey );
            s3Config = new AmazonS3Config
            {
                ServiceURL = options.ServiceUrl
            };
        }

        public IJsonDb GetJsonDb()
        {
            return new S3JsonDb( new AmazonS3Client( credentials, s3Config )
                , options.BucketName
                , options.DbPath
                , options.CollectionAdapter
                , options.JsonSerializerOptions );
        }
    }
}
