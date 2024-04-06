using DevDoListServer.Models.Dtos;
using DevDoListServer.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
        public async Task<ActionResult<IEnumerable<StatusDto>>> GetStatuses()
        {
            var statuses = await _statusRepository.GetAll();
            var statusDtos = statuses.Select(status => new StatusDto(status)).ToList();
            return Ok(statusDtos);
        }

        [HttpGet("{id}")]
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
        public async Task<ActionResult<StatusDto>> PostStatus([FromBody] StatusCreateDto status)
        {
            var createdStatus = await _statusRepository.Create(status.ToStatus());
            var statusDto = new StatusDto(createdStatus);
            return CreatedAtAction("GetStatus", new { id = statusDto.StatusId }, statusDto);
        }
        
        [HttpDelete("{id}")]
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
