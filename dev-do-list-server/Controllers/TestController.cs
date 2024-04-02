using DevDoListServer.Services;
using Microsoft.AspNetCore.Mvc;

namespace DevDoListServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        private readonly TestService testService;
        public TestController()
        {
            testService = new TestService();   
        }

        [HttpGet("Test/{id}")]
        public ActionResult<Hello> GetAuthWhatEver([FromRoute] int id)
        {
            try
            {
               return testService.TestServiceFunc(id);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

    }

    public class Hello { }
}




