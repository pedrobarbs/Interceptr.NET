namespace TheInterceptor
{
    public interface IInterceptor
    {
        public void ExecuteBefore(CallContext context);
        public void ExecuteAfter(CallContext context, object result);
    }

    public class Teste
    {
        public IInterceptor[] interceptors;

        public void ExecuteInterceptors() 
        {
            foreach (var interceptor in interceptors.Reverse())
            {
                interceptor.ExecuteBefore(null);
            }
        }
    }
}
