
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Reflection;

namespace Interceptr
{
    public static class IServiceCollectionExtensions
    {
        /// <summary>
        /// Add a transient service and intercepts them. The first provided interceptor will execute closer to the service's method. So the last provided interceptor will execute more distant to the service's method.
        /// </summary>
        /// <typeparam name="Interface"></typeparam>
        /// <typeparam name="Class"></typeparam>
        /// <param name="services"></param>
        /// <param name="interceptors"></param>
        /// <exception cref="InvalidOperationException"></exception>
        public static void AddTransientIntercepted<Interface, Class>(this IServiceCollection services, params IInterceptor[] interceptors) where Class : class
        {
            services.TryAddTransient<Class>();

            var assembly = Assembly.GetCallingAssembly();

            var intercepted = GetInterceptedType<Class>(assembly);

            services.AddTransient(typeof(Interface), provider =>
            {
                return CreateInstance<Class>(interceptors, provider, intercepted);
            });
        }

        /// <summary>
        /// Add a scoped service and intercepts them. The first provided interceptor will execute closer to the service's method. So the last provided interceptor will execute more distant to the service's method.
        /// </summary>
        /// <typeparam name="Interface"></typeparam>
        /// <typeparam name="Class"></typeparam>
        /// <param name="services"></param>
        /// <param name="interceptors"></param>
        /// <exception cref="InvalidOperationException"></exception>
        public static void AddScopedIntercepted<Interface, Class>(this IServiceCollection services, params IInterceptor[] interceptors) where Class : class
        {
            services.TryAddScoped<Class>();

            var assembly = Assembly.GetCallingAssembly();

            var intercepted = GetInterceptedType<Class>(assembly);

            services.AddScoped(typeof(Interface), provider =>
            {
                return CreateInstance<Class>(interceptors, provider, intercepted);
            });
        }

        /// <summary>
        /// Add a singleton service and intercepts them. The first provided interceptor will execute closer to the service's method. So the last provided interceptor will execute more distant to the service's method.
        /// </summary>
        /// <typeparam name="Interface"></typeparam>
        /// <typeparam name="Class"></typeparam>
        /// <param name="services"></param>
        /// <param name="interceptors"></param>
        /// <exception cref="InvalidOperationException"></exception>
        public static void AddSingletonIntercepted<Interface, Class>(this IServiceCollection services, params IInterceptor[] interceptors) where Class : class
        {
            services.TryAddSingleton<Class>();

            var assembly = Assembly.GetCallingAssembly();

            var intercepted = GetInterceptedType<Class>(assembly);

            services.AddSingleton(typeof(Interface), provider =>
            {
                return CreateInstance<Class>(interceptors, provider, intercepted);
            });
        }

        private static object CreateInstance<Class>(IInterceptor[] interceptors, IServiceProvider provider, Type intercepted) where Class : class
        {
            var service = provider.GetService<Class>();

            return Activator.CreateInstance(intercepted, service, interceptors);
        }

        private static Type GetInterceptedType<Class>(Assembly assembly) where Class : class
        {
            var classname = typeof(Class).Name;

            var intercepted = assembly.GetType($"{classname}Intercepted", true);

            if (intercepted is null)
                throw new InvalidOperationException($"{classname} has no dynamically generated intercepted service");
            return intercepted;
        }
    }
}
