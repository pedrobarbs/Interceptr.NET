namespace TheInterceptor
{
    public static class IServiceCollectionExtensions
    {
        public static void AddScopedIntercepted<Interface, Class>(this IServiceCollection services) 
        {
            //services.Add<Interface, InterceptionService>();  
        
        }
    }
}
