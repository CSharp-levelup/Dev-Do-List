using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace DevDoListServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        [HttpGet("Test/{id}")]
        [ProducesResponseType<string>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult GetAuthWhatEver([FromRoute] int id)
        {
            try
            {
/*                throw new NotImplementedException();*/
                if (id == 0)
                    return Ok("Good Response");

                if (id == 1)
                    return NotFound();

                return Conflict("Not good");
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "asdasdd");
            }
        }

        [HttpGet("Test2/{id}")]
        public Results<Ok<string>, NotFound, Conflict<string>, StatusCodeHttpResult, Created> GetAuthWhatEver2([FromRoute] int id)
        {
            //return Results.Ok("Good Response");
            try
            {
                /*                throw new NotImplementedException();*/
                if (id == 0)
                    return TypedResults.Ok("Good Response");

                if (id == 1)
                    return TypedResults.NotFound();

                return TypedResults.Conflict("Not good");

            }
            catch (Exception)
            {
                return TypedResults.StatusCode(500);
            }
        }

    }
}


