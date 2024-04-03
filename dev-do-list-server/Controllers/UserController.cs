using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DevDoListServer.Models;
using DevDoListServer.Repositories;

namespace DevDoListServer.Controllers
{
    [Route("api/v1/user")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserRepository _userRepository;

        public UserController(UserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
            return await _userRepository.GetAll();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUser([FromRoute] int id)
        {
            var user = await _userRepository.GetById(id);

            if (user == null)
            {
                return NotFound("User not found");
            }

            return Ok(user);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutUser([FromRoute] int id, [FromBody] User user)
        {
            if (id != user.UserId)
            {
                return BadRequest();
            }

            
            try
            {
                await _userRepository.Update(user);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_userRepository.UserExists(id))
                {
                    return NotFound("User not found");
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        [HttpPost]
        public async Task<ActionResult<User>> PostUser([FromBody] User user)
        {
            var createdUser = await _userRepository.Create(user);

            return CreatedAtAction("GetUser", new { id = createdUser.UserId }, createdUser);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser([FromRoute] int id)
        {
            var user = await _userRepository.GetById(id);

            if (user == null)
            {
                return NotFound("User not found");
            }

            await _userRepository.Delete(user);

            return NoContent();
        }

    }
}
