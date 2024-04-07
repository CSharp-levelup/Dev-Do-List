using DevDoListServer.Jwt;
using DevDoListServer.Models;
using DevDoListServer.Models.Dtos;
using DevDoListServer.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.Annotations;

namespace DevDoListServer.Controllers
{
    [Route("api/v1/user")]
    [ApiController]
    public class UserController(UserRepository userRepository) : ControllerBase
    {
        [HttpGet]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(IEnumerable<UserDto>))]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetUsers()
        {
            var users = await userRepository.GetAll();
            return users.Select(user => new UserDto(user)).ToList();
        }

        [HttpGet("{id}")]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(UserDto))]
        public async Task<ActionResult<UserDto>> GetUser([FromRoute] int id)
        {
            var user = await userRepository.GetById(id);

            if (user == null)
            {
                return NotFound("User not found");
            }

            return Ok(new UserDto(user));
        }
        [HttpGet("loggedIn")]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(UserDto))]
        public async Task<ActionResult<UserDto>> GetUserByUsername([FromHeader(Name = "Authorization")] string authToken)
        {
            var username = JwtUtils.GetClaim(authToken, "username");
            var user = await userRepository.FindByUsername(username);

            if (user is null)
            {
                return NotFound("User not found");
            }

            return Ok(new UserDto(user));
        }
        
        [HttpPut("{id}")]
        [SwaggerResponse(StatusCodes.Status204NoContent)]
        [SwaggerResponse(StatusCodes.Status401Unauthorized)]
        [SwaggerResponse(StatusCodes.Status404NotFound)]
        [SwaggerResponse(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> PutUser([FromRoute] int id, [FromBody] UserDto userDto)
        {
            if (id != userDto.UserId)
            {
                return BadRequest();
            }

            
            try
            {
                await userRepository.Update(userDto.ToUser());
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await userRepository.Exists(e => e.UserId == id))
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
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(UserDto))]
        public async Task<ActionResult<User>> PostUser([FromBody] UserCreateDto userCreateDto)
        {
            var createdUser = await userRepository.Create(userCreateDto.ToUser());
            var userDto = new UserDto(createdUser);
            return CreatedAtAction("GetUser", new { id = userDto.UserId }, userDto);
        }

        [HttpDelete("{id}")]
        [SwaggerResponse(StatusCodes.Status204NoContent)]
        [SwaggerResponse(StatusCodes.Status401Unauthorized)]
        [SwaggerResponse(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteUser([FromRoute] int id)
        {
            var user = await userRepository.GetById(id);

            if (user == null)
            {
                return NotFound("User not found");
            }

            await userRepository.Delete(user);

            return NoContent();
        }

    }
}
