namespace TheInterceptor
{
    public class SampleInterceptor : IInterceptor
    {
        public void Pre()
        {
            System.Diagnostics.Debug.WriteLine("Starting");
            Console.Out.WriteLine(new DateTime().ToLongDateString());
        }

        public void Pos()
        {
            System.Diagnostics.Debug.WriteLine("Finishing");
            Console.Out.WriteLine(new DateTime().ToLongDateString());
        }
    }
}
