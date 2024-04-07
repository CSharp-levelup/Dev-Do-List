using DevDoListServer.Jwt;
using DevDoListServer.Models.Dtos;
using DevDoListServer.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.Annotations;

namespace DevDoListServer.Controllers
{
    [Route("api/v1/task")]
    [ApiController]
    public class TaskController(TaskRepository taskRepository) : ControllerBase
    {
        [HttpGet]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(IEnumerable<TaskResponseDto>))]
        [SwaggerResponse(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<TaskResponseDto>> GetTasks([FromHeader(Name = "Authorization")] string authToken)
        {
            var username = JwtUtils.GetClaim(authToken, "username");
            var tasks = await taskRepository.FindAll(task => task.User.Username == username);
            var dtos = tasks.Select(t => new TaskResponseDto(t));
            return Ok(dtos);
        }
        
        [HttpGet("{id}")]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(TaskResponseDto))]
        [SwaggerResponse(StatusCodes.Status401Unauthorized)]
        [SwaggerResponse(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<TaskResponseDto>> GetTask([FromRoute] int id)
        {
            var task = await taskRepository.GetById(id);

            if (task == null)
            {
                return NotFound("Task not found");
            }

            return Ok(new TaskResponseDto(task));
        }
        
        [HttpPut("{id}")]
        [SwaggerResponse(StatusCodes.Status204NoContent)]
        [SwaggerResponse(StatusCodes.Status401Unauthorized)]
        [SwaggerResponse(StatusCodes.Status404NotFound)]
        [SwaggerResponse(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> PutTask([FromRoute] int id, [FromBody] TaskUpdateDTO taskUpdateDto)
        {
            if (id != taskUpdateDto.TaskId)
            {
                return BadRequest();
            }

            
            try
            {
                await taskRepository.Update(taskUpdateDto.ToTask());
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await taskRepository.Exists(e => e.TaskId == id))
                {
                    return NotFound("Task not found");
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        [HttpPost]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(TaskResponseDto))]
        public async Task<ActionResult<TaskResponseDto>> PostTask([FromBody] TaskCreateDto taskCreateDto)
        {
            var createdTask = await taskRepository.Create(taskCreateDto.ToTask());
            var taskDto = new TaskResponseDto(createdTask);
            return CreatedAtAction("GetTask", new { id = taskDto.UserId }, taskDto);
        }

        [HttpDelete("{id}")]
        [SwaggerResponse(StatusCodes.Status204NoContent)]
        [SwaggerResponse(StatusCodes.Status401Unauthorized)]
        [SwaggerResponse(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteTask(int id)
        {
            var task = await taskRepository.GetById(id);

            if (task == null)
            {
                return NotFound("Task not found");
            }

            await taskRepository.Delete(task);

            return NoContent();
        }
    }
}
