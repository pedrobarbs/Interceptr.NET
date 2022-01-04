
namespace TheInterceptor
{
    public class SampleInterceptor : IInterceptor
    {
        public void ExecuteBefore()
        {
            Print("Starting");
        }

        public void ExecuteAfter()
        {
            Print("Finishing");
        }

        private static void Print(string @string)
        {
            System.Diagnostics.Debug.WriteLine(@string);
        }
    }
}
