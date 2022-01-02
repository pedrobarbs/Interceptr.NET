namespace TheInterceptor
{
    public class SampleInterceptor : IInterceptor
    {
        public void Pre()
        {
            Console.WriteLine("Starting");
            Console.WriteLine(new DateTime().ToLongDateString());
        }

        public void Pos()
        {
            Console.WriteLine("Finishing");
            Console.WriteLine(new DateTime().ToLongDateString());
        }
    }
}
