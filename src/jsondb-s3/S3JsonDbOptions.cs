using System;

namespace JsonDb.S3
{
    public class S3JsonDbOptions : JsonDb.JsonDbOptions
    {
        public string ServiceUrl { get; set; }
        public string AccessKey { get; set; }
        public string SecretKey { get; set; }
        public string BucketName { get; set; }
        public string DbPath { get; set; }
    }
}
