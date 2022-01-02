namespace TheInterceptor
{
    public class SampleInterceptor : IInterceptor
    {
        public void Pre()
        {
            Print("Starting");
        }

        public void Pos()
        {
            Print("Finishing");
        }

        private static void Print(string @string)
        {
            System.Diagnostics.Debug.WriteLine(@string);
        }
    }
}
