using Microsoft.Extensions.DependencyInjection;
using Interceptr.Sample.Layer2;
using Interceptr.Sample.Layer3;

namespace Interceptr.Sample.IOC
{
    public static class IServiceCollectionExtensions
    {
        public static void AddOtherLayers(this IServiceCollection services)
        {
            services.AddTransientIntercepted<IServiceLayer2, ServiceLayer2>(
                new ChronometerInterceptor2()
                );

            services.AddScopedIntercepted<IServiceLayer3, ServiceLayer3>(
                new ChronometerInterceptor2()
                );
        }
    }
}