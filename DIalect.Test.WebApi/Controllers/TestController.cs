using Microsoft.AspNetCore.Mvc;

namespace DIalect.Test.WebApi.Controllers
{
    [Route("api/test")]
    [ApiController]
    public class TestController(ITestService service) : Controller
    {
        private readonly ITestService _service = service;

        [HttpGet("message")]
        public IActionResult GetMessage()
        {
            return Ok(_service.GetHelloMessage());
        }

        [HttpGet("messages")]
        public IActionResult GetMessages()
        {
            return Ok(_service.GetMessages());
        }
    }
}
