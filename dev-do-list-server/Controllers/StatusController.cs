using DevDoListServer.Models.Dtos;
using DevDoListServer.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.Annotations;

namespace DevDoListServer.Controllers
{
    [Route("api/v1/status")]
    [ApiController]
    public class StatusController : ControllerBase
    {
        private readonly StatusRepository _statusRepository;

        public StatusController(StatusRepository statusRepository)
        {
            _statusRepository = statusRepository;
        }

        [HttpGet]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(IEnumerable<StatusDto>))]
        [SwaggerResponse(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<IEnumerable<StatusDto>>> GetStatuses()
        {
            var statuses = await _statusRepository.GetAll();
            var statusDtos = statuses.Select(status => new StatusDto(status)).ToList();
            return Ok(statusDtos);
        }

        [HttpGet("{id}")]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(StatusDto))]
        [SwaggerResponse(StatusCodes.Status401Unauthorized)]
        [SwaggerResponse(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<StatusDto>> GetStatus([FromRoute] int id)
        {
            var status = await _statusRepository.GetById(id);

            if (status == null)
            {
                return NotFound("Status not found");
            }

            return new StatusDto(status);
        }

        [HttpPut("{id}")]
        [SwaggerResponse(StatusCodes.Status204NoContent)]
        [SwaggerResponse(StatusCodes.Status401Unauthorized)]
        [SwaggerResponse(StatusCodes.Status404NotFound)]
        [SwaggerResponse(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> PutStatus([FromRoute] int id, [FromBody] StatusDto status)
        {
            if (id != status.StatusId)
            {
                return BadRequest();
            }

            try
            {
                await _statusRepository.Update(status.ToStatus());
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _statusRepository.Exists(e => e.StatusId == id))
                {
                    return NotFound("Status not found");
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        [HttpPost]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(StatusDto))]
        public async Task<ActionResult<StatusDto>> PostStatus([FromBody] StatusCreateDto status)
        {
            var createdStatus = await _statusRepository.Create(status.ToStatus());
            var statusDto = new StatusDto(createdStatus);
            return CreatedAtAction("GetStatus", new { id = statusDto.StatusId }, statusDto);
        }
        
        [HttpDelete("{id}")]
        [SwaggerResponse(StatusCodes.Status204NoContent)]
        [SwaggerResponse(StatusCodes.Status401Unauthorized)]
        [SwaggerResponse(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteStatus([FromRoute] int id)
        {
            var status = await _statusRepository.GetById(id);
            if (status == null)
            {
                return NotFound("Status not found");
            }

            await _statusRepository.Delete(status);

            return NoContent();
        }
    }
}
