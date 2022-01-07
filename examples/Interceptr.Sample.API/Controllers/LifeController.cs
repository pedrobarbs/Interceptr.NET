using Microsoft.AspNetCore.Mvc;
using Interceptr.Entities;
using Interceptr.Interfaces;
using Interceptr.Sample.Layer2;
using System.Threading.Tasks;

namespace Interceptr.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class LifeController : ControllerBase
    {
        private readonly ISampleService _sampleService;
        private readonly IServiceLayer2 _layer2;

        public LifeController(ISampleService sampleService, IServiceLayer2 layer2)
        {
            _layer2 = layer2;
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

        [HttpGet("layer")]
        public async Task<IActionResult> Layer()
        {
            return Ok(_layer2.GetLuckyNumber());
        }
    }
}