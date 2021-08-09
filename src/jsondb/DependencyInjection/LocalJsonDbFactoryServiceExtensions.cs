using System;
using JsonDb;
using JsonDb.Local;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class LocalJsonDbFactoryServiceExtensions
    {
        public static IServiceCollection AddLocalJsonDb( this IServiceCollection services, Action<LocalJsonDbOptions> configure )
        {
            services.AddSingleton<IJsonDbFactory, LocalJsonDbFactory>()
                .Configure( configure );

            return ( services );
        }
    }
}
