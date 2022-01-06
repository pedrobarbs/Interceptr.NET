using Microsoft.Extensions.DependencyInjection;
using TheInterceptor.Sample.Layer2;
using TheInterceptor.Sample.Layer3;

namespace TheInterceptor.Sample.IOC
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