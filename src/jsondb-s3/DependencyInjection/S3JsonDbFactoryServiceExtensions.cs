using System;
using JsonDb;
using JsonDb.S3;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class S3JsonDbFactoryServiceExtensions
    {
        public static IServiceCollection AddS3JsonDb( this IServiceCollection services, Action<S3JsonDbOptions> configure )
        {
            services.AddSingleton<IJsonDbFactory, S3JsonDbFactory>()
                .Configure( configure );

            return ( services );
        }
    }
}
