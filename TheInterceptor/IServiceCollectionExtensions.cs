namespace TheInterceptor
{
    public static class IServiceCollectionExtensions
    {
        public static void AddScopedIntercepted<Interface, Class>(this IServiceCollection services) where Class : class
        {
            services.AddScoped<Class>();  
            services.AddScoped<ISampleService, SampleServiceInterceptor>();  
        
        }
    }
}
