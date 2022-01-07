using System.Diagnostics;

namespace Interceptr.Sample.Layer2
{
    public class ChronometerInterceptor2 : IInterceptor
    {
        private Stopwatch _sw;

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
