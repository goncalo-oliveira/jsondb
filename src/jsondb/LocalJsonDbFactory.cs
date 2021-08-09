using System;
using Microsoft.Extensions.Options;

namespace JsonDb.Local
{
    internal class LocalJsonDbFactory : IJsonDbFactory
    {
        private readonly LocalJsonDbOptions options;

        public LocalJsonDbFactory( IOptions<LocalJsonDbOptions> optionsAccessor )
        {
            options = optionsAccessor.Value;
        }

        public IJsonDb GetJsonDb()
        {
            return new LocalJsonDb( options.DbPath, options.JsonSerializerOptions );
        }
    }
}
