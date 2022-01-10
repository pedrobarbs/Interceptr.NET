// Copyright 2022 - Interceptr.NET - https://github.com/pedrobarbs/Interceptr.NET
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Linq;
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
        public static void AddTransientIntercepted<Interface, Class>(this IServiceCollection services, params IInterceptr[] interceptors)
            where Class : class, Interface
        {
            var assembly = Assembly.GetCallingAssembly();
            AddTransientIntercepted<Interface, Class>(services, assembly, interceptors);
        }

        /// <summary>
        /// Add a transient service and intercepts them with a stack of interceptors.
        /// </summary>
        /// <typeparam name="Interface"></typeparam>
        /// <typeparam name="Class"></typeparam>
        /// <param name="services"></param>
        /// <param name="interceptors"></param>
        /// <exception cref="InvalidOperationException"></exception>
        public static void AddTransientIntercepted<Interface, Class>(this IServiceCollection services, InterceptrStack interceptors)
            where Class : class, Interface
        {
            var assembly = Assembly.GetCallingAssembly();
            services.AddTransientIntercepted<Interface, Class>(assembly, interceptors._interceptors);
        }

        /// <summary>
        /// Add a transient service and intercepts them. The first provided interceptor will execute closer to the service's method. So the last provided interceptor will execute more distant to the service's method.
        /// </summary>
        /// <typeparam name="Interface"></typeparam>
        /// <typeparam name="Class"></typeparam>
        /// <param name="services"></param>
        /// <param name="assembly"></param>
        /// <param name="interceptors"></param>
        /// <exception cref="InvalidOperationException"></exception>
        private static void AddTransientIntercepted<Interface, Class>(this IServiceCollection services, Assembly assembly, params IInterceptr[] interceptors)
            where Class : class, Interface
        {
            services.TryAddTransient<Class>();

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
        public static void AddScopedIntercepted<Interface, Class>(this IServiceCollection services, params IInterceptr[] interceptors)
            where Class : class, Interface
        {
            var assembly = Assembly.GetCallingAssembly();
            AddScopedIntercepted<Interface, Class>(services, assembly, interceptors);
        }

        /// <summary>
        /// Add a scoped service and intercepts them with a stack of interceptors.
        /// </summary>
        /// <typeparam name="Interface"></typeparam>
        /// <typeparam name="Class"></typeparam>
        /// <param name="services"></param>
        /// <param name="interceptors"></param>
        /// <exception cref="InvalidOperationException"></exception>
        public static void AddScopedIntercepted<Interface, Class>(this IServiceCollection services, InterceptrStack interceptors)
            where Class : class, Interface
        {
            var assembly = Assembly.GetCallingAssembly();
            services.AddScopedIntercepted<Interface, Class>(assembly, interceptors._interceptors);
        }

        /// <summary>
        /// Add a scoped service and intercepts them. The first provided interceptor will execute closer to the service's method. So the last provided interceptor will execute more distant to the service's method.
        /// </summary>
        /// <typeparam name="Interface"></typeparam>
        /// <typeparam name="Class"></typeparam>
        /// <param name="services"></param>
        /// <param name="assembly"></param>
        /// <param name="interceptors"></param>
        /// <exception cref="InvalidOperationException"></exception>
        private static void AddScopedIntercepted<Interface, Class>(this IServiceCollection services, Assembly assembly, params IInterceptr[] interceptors)
            where Class : class, Interface
        {
            services.TryAddScoped<Class>();

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
        public static void AddSingletonIntercepted<Interface, Class>(this IServiceCollection services, params IInterceptr[] interceptors)
            where Class : class, Interface
        {
            var assembly = Assembly.GetCallingAssembly();
            AddSingletonIntercepted<Interface, Class>(services, assembly, interceptors);
        }

        /// <summary>
        /// Add a singleton service and intercepts them with a stack of interceptors.
        /// </summary>
        /// <typeparam name="Interface"></typeparam>
        /// <typeparam name="Class"></typeparam>
        /// <param name="services"></param>
        /// <param name="interceptors"></param>
        /// <exception cref="InvalidOperationException"></exception>
        public static void AddSingletonIntercepted<Interface, Class>(this IServiceCollection services, InterceptrStack interceptors)
            where Class : class, Interface
        {
            var assembly = Assembly.GetCallingAssembly();
            services.AddSingletonIntercepted<Interface, Class>(assembly, interceptors._interceptors);
        }

        /// <summary>
        /// Add a singleton service and intercepts them. The first provided interceptor will execute closer to the service's method. So the last provided interceptor will execute more distant to the service's method.
        /// </summary>
        /// <typeparam name="Interface"></typeparam>
        /// <typeparam name="Class"></typeparam>
        /// <param name="services"></param>
        /// <param name="assembly"></param>
        /// <param name="interceptors"></param>
        /// <exception cref="InvalidOperationException"></exception>
        private static void AddSingletonIntercepted<Interface, Class>(this IServiceCollection services, Assembly assembly, params IInterceptr[] interceptors)
            where Class : class, Interface
        {
            services.TryAddSingleton<Class>();

            var intercepted = GetInterceptedType<Class>(assembly);

            services.AddSingleton(typeof(Interface), provider =>
            {
                return CreateInstance<Class>(interceptors, provider, intercepted);
            });
        }

        private static object CreateInstance<Class>(IInterceptr[] interceptors, IServiceProvider provider, Type intercepted) where Class : class
        {
            var interceptorList = interceptors.ToList();

#if RELEASE
            interceptorList.RemoveAll(
                interceptor => interceptor.GetStatus() is InterceptrStatus.Disabled ||
                interceptor.GetStatus() is InterceptrStatus.EnabledWhenDebugging);
#else
            interceptorList.RemoveAll(interceptor => interceptor.GetStatus() is InterceptrStatus.Disabled);
#endif

            var service = provider.GetService<Class>();

            return Activator.CreateInstance(intercepted, service, interceptorList.ToArray());
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
