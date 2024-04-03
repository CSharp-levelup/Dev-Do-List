using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DevDoListServer.Data;
using DevDoListServer.Models;
using DevDoListServer.Repositories;

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
        public async Task<ActionResult<IEnumerable<Status>>> GetStatuses()
        {
            return await _statusRepository.GetAll();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Status>> GetStatus([FromRoute] int id)
        {
            var status = await _statusRepository.GetById(id);

            if (status == null)
            {
                return NotFound("Status not found");
            }

            return status;
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutStatus([FromRoute] int id, [FromBody] Status status)
        {
            if (id != status.StatusId)
            {
                return BadRequest();
            }

            try
            {
                await _statusRepository.Update(status);
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
        public async Task<ActionResult<Status>> PostStatus([FromBody] Status status)
        {
            var createdStatus = await _statusRepository.Create(status);

            return CreatedAtAction("GetStatus", new { id = createdStatus.StatusId }, createdStatus);
        }

        // DELETE: api/Status/5
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
