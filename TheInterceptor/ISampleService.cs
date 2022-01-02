namespace TheInterceptor
{
    public interface ISampleService
    {
        Task CreateLifeAsync();
        void CreateLife();
        Task<bool> IsLifeCreated();
        int GetMeaningOfLife(bool teste);
    }
}