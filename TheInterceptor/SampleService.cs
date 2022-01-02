namespace TheInterceptor
{
    public class SampleService : ISampleService
    {
        public Task CreateLifeAsync() => Task.CompletedTask;
        public void CreateLife() => Console.WriteLine();

        public async Task<bool> IsLifeCreated() {
            await Task.Delay(5000);
            return await Task.FromResult(true);
        }

        public int GetMeaningOfLife(bool teste) => 42;
    }
}
