namespace TheInterceptor
{
    public static class IServiceCollectionExtensions
    {
        public static void AddScopedIntercepted<Interface, Class>(this IServiceCollection services, IInterceptor interceptor) where Class : class
        {
            services.AddScoped<Class>();  
            services.AddScoped<ISampleService, SampleServiceIntercepted>(provider => 
            { 
                var service = provider.GetService<Class>();
                new SampleServiceIntercepted(service, interceptor);
            });  
        }
    }
}
