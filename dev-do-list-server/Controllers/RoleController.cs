using DevDoListServer.Models;
using DevDoListServer.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DevDoListServer.Controllers
{
    [Route("api/v1/role")]
    [ApiController]
    public class RoleController(RoleRepository roleRepository) : ControllerBase
    {
        private readonly RoleRepository roleRepository = roleRepository;

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Role>>> GetRoles()
        {
            return await roleRepository.GetAll();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Role>> GetStatus([FromRoute] int id)
        {
            var role = await roleRepository.GetById(id);

            if (role == null)
            {
                return NotFound("Role not found");
            }

            return role;
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutRole([FromRoute] int id, [FromBody] Role role)
        {
            if (id != role.RoleId)
            {
                return BadRequest();
            }

            try
            {
               return Ok(await roleRepository.Update(role));
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await roleRepository.Exists(e => e.RoleId == id))
                {
                    return NotFound("Role not found");
                }
                else
                {
                    throw;
                }
            }

        }

        [HttpPost]
        public async Task<ActionResult<Status>> PostRole([FromBody] Role role)
        {
            var createdRole = await roleRepository.Create(role);

            return CreatedAtAction("GetRole", new { id = createdRole.RoleId }, createdRole);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteStatus([FromRoute] int id)
        {
            var role = await roleRepository.GetById(id);
            if (role == null)
            {
                return NotFound("Role not found");
            }

            await roleRepository.Delete(role);

            return NoContent();
        }
    }
}
