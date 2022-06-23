using Microsoft.AspNetCore.Mvc;
using OkxPerpetualArbitrage.Application.Exceptions;
using OkxPerpetualArbitrage.Application.Models.DTOs;

namespace OkxPerpetualArbitrage.WebApi.Controllers
{
    public class TestController : ControllerBase
    {
        private readonly ILogger<TestController> logger;

        public TestController(ILogger<TestController> logger)
        {
            this.logger = logger;
        }

    

        [HttpGet]
        [Route("api/test")]
        public IActionResult Test(TestDto testDto)
        {
            //   throw new OkxPerpetualArbitrageCustomException("ex error");
            // logger.LogInformation("hello");
            var v = 10 / testDto.Age;
            if(!ModelState.IsValid)
                return BadRequest(ModelState);
            return Ok("Test " + testDto.Name);
        }
    }
}
