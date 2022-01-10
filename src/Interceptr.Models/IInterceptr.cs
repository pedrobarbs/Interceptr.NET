using Interceptr;

namespace Interceptr
{
    public interface IInterceptr
    {
        InterceptrStatus GetStatus();

        void ExecuteBefore(CallContext context);
        void ExecuteAfter(CallContext context, object result);
    }
}
