using TheInterceptor.Sample.Layer3;

namespace TheInterceptor.Sample.Layer2
{
    public class ServiceLayer2 : IServiceLayer2
    {
        private readonly IServiceLayer3 _serviceLayer3;

        public ServiceLayer2(IServiceLayer3 serviceLayer3)
        {
            _serviceLayer3 = serviceLayer3;
        }

        public int GetLuckyNumber()
        {
            return _serviceLayer3.GetLuckyNumber();
        }
    }
}