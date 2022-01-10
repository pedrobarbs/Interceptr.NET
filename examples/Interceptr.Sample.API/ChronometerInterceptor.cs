using System.Diagnostics;
using Interceptr;

namespace Interceptr
{
    public class ChronometerInterceptor : IInterceptr
    {
        private Stopwatch _sw;

        public InterceptrStatus GetStatus() => InterceptrStatus.EnabledWhenDebugging;

        public void ExecuteBefore(CallContext context)
        {
            _sw = Stopwatch.StartNew();
        }

        public void ExecuteAfter(CallContext context, object result)
        {
            _sw.Stop();
            Print($"{_sw.Elapsed.TotalMilliseconds} milliseconds to execute");
        }


        private static void Print(string @string)
        {
            System.Diagnostics.Debug.WriteLine(@string);
        }
    }
}
