using System;
using System.Threading.Tasks;
using Interceptr.Entities;
using Interceptr.Interfaces;

namespace Interceptr.Services
{
    public class SampleService : ISampleService
    {
        public Task CreateLifeAsync() => Task.CompletedTask;
        public void CreateLife() => Console.WriteLine();

        public async Task<bool> IsLifeCreated(Life life) {
            await Task.Delay(5000);
            return await Task.FromResult(life != null);
        }

        public int GetMeaningOfLife(bool teste) => 42;

        public async Task<Life> GetOlder(Life life)
        {
            return await Task.FromResult(life);
        }
    }
}
