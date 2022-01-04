namespace TheInterceptor
{
    public interface IInterceptor
    {
        public void ExecuteBefore(CallContext context);
        public void ExecuteAfter(CallContext context, object result);
    }
}
