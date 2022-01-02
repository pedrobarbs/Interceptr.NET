namespace TheInterceptor
{
    public interface IInterceptor
    {
        public void ExecuteBefore();
        public void ExecuteAfter();
    }
}
