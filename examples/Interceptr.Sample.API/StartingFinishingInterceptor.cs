using Interceptr;


namespace Interceptr
{
    public class StartingFinishingInterceptor : IInterceptr
    {
        public InterceptrStatus GetStatus() => InterceptrStatus.Enabled;

        public void ExecuteBefore(CallContext context)
        {
            Print("Starting");
        }

        public void ExecuteAfter(CallContext context, object result)
        {
            Print("Finishing");
        }

        private static void Print(string @string)
        {
            System.Diagnostics.Debug.WriteLine(@string);
        }
    }
}
