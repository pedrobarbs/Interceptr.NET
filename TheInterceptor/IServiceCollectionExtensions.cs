namespace TheInterceptor
{
    public static class IServiceCollectionExtensions
    {
        public static void AddScopedIntercepted<Interface, Class>(this IServiceCollection services, IInterceptor interceptor) where Class : class
        {
            services.AddScoped<Class>();

            var classname = typeof(Class).Name;
            var intercepted = Type.GetType($"{classname}Intercepted", true);

            if (intercepted is null)
                throw new InvalidOperationException($"{classname} has no dynamically generated intercepted service");

            services.AddScoped(typeof(Interface), provider => 
            {
                var service = provider.GetService<Class>()!;

                return Activator.CreateInstance(intercepted, service, interceptor)!;
            });
        }
    }
}
