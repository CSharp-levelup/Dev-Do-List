using DevDoListServer.Models;
using DevDoListServer.Models.Dtos;
using DevDoListServer.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
        public async Task<ActionResult<IEnumerable<UserDto>>> GetUsers()
        {
            var users = await _userRepository.GetAll();
            return users.Select(user => new UserDto(user)).ToList();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<UserDto>> GetUser([FromRoute] int id)
        {
            var user = await _userRepository.GetById(id);

            if (user == null)
            {
                return NotFound("User not found");
            }

            return Ok(new UserDto(user));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutUser([FromRoute] int id, [FromBody] UserDto userDto)
        {
            if (id != userDto.UserId)
            {
                return BadRequest();
            }

            
            try
            {
                await _userRepository.Update(userDto.ToUser());
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _userRepository.Exists(e => e.UserId == id))
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
        [Authorize(Roles = RoleType.Admin)]
        public async Task<ActionResult<User>> PostUser([FromBody] UserCreateDto userCreateDto)
        {
            var createdUser = await _userRepository.Create(userCreateDto.ToUser());
            var userDto = new UserDto(createdUser);
            return CreatedAtAction("GetUser", new { id = userDto.UserId }, userDto);
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
