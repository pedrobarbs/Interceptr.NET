using Microsoft.AspNetCore.Mvc;
using TheInterceptor.Entities;
using TheInterceptor.Interfaces;

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

        [HttpGet("Meaning")]
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
            var life = new Life();
            var result = await _sampleService.IsLifeCreated(life);
            return Ok(result ? "Yeah, there is LIFE!" : "No, there is no life here");
        }
    }
}