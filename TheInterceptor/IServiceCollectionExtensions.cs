
using Microsoft.Extensions.DependencyInjection;
using System;

namespace TheInterceptor.SourceGenerator
{
    public static class IServiceCollectionExtensions
    {
        /// <summary>
        /// Add scoped services and intercepts them. The first provided interceptor will execute closer to the service's method. So the last provided interceptor will execute more distant to the service's method.
        /// </summary>
        /// <typeparam name="Interface"></typeparam>
        /// <typeparam name="Class"></typeparam>
        /// <param name="services"></param>
        /// <param name="interceptors"></param>
        /// <exception cref="InvalidOperationException"></exception>
        public static void AddScopedIntercepted<Interface, Class>(this IServiceCollection services, params IInterceptor[] interceptors) where Class : class
        {
            services.AddScoped<Class>();

            var classname = typeof(Class).Name;
            var intercepted = Type.GetType($"{classname}Intercepted", true);

            if (intercepted is null)
                throw new InvalidOperationException($"{classname} has no dynamically generated intercepted service");

            services.AddScoped(typeof(Interface), provider => 
            {
                var service = provider.GetService<Class>();

                return Activator.CreateInstance(intercepted, service, interceptors);
            });
        }
    }
}
