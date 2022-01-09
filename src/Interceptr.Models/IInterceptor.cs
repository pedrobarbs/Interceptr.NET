namespace Interceptr
{
    public interface IInterceptor
    {
        bool DebugOnly();

        void ExecuteBefore(CallContext context);
        void ExecuteAfter(CallContext context, object result);
    }
}
