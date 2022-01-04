using TheInterceptor.Entities;

namespace TheInterceptor.Interfaces
{
    public interface ISampleService
    {
        Task CreateLifeAsync();
        void CreateLife();
        Task<bool> IsLifeCreated(Life life);
        Task<Life> GetOlder(Life life);
        int GetMeaningOfLife(bool teste);
    }
}