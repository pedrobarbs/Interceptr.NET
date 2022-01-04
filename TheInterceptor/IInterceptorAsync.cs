namespace TheInterceptor
{
    public interface IInterceptorAsync
    {
        public Task ExecuteBeforeAsync(CallContext context);
        public Task ExecuteAfterAsync();
    }
}
