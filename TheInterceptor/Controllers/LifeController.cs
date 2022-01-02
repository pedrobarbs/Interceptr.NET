using Microsoft.AspNetCore.Mvc;

namespace TheInterceptor.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class LifeController : ControllerBase
    {
        private readonly ISampleService _sampleService;

        public LifeController(ISampleService sampleService)
        {
            _sampleService = sampleService;
        }

        [HttpGet]
        public string Get() => _sampleService.GetMeaningOfLife(true).ToString();
    }
}