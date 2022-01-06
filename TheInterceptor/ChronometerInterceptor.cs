﻿using System.Diagnostics;
using TheInterceptor;

namespace TheInterceptor
{
    public class ChronometerInterceptor : IInterceptor
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