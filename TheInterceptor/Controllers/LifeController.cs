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
        public string Get()
        {
            return _sampleService.GetMeaningOfLife(true).ToString();
        }

        [HttpGet("Create")]
        public async Task<IActionResult> Create()
        {
            await _sampleService.CreateLifeAsync();
            return Ok("Life apparently created..");
        }

        [HttpGet("Created")]
        public async Task<IActionResult> Created()
        {
            var result = await _sampleService.IsLifeCreated();
            return Ok(result ? "Yeah, there is LIFE!" : "No, there is no life here");
        }
    }
}