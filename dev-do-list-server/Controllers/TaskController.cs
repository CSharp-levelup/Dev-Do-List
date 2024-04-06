using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DevDoListServer.Models;
using DevDoListServer.Repositories;
using DevDoListServer.Jwt;

namespace DevDoListServer.Controllers
{
    [Route("api/v1/task")]
    [ApiController]
    public class TaskController : ControllerBase
    {
        private readonly TaskRepository _taskRepository;
        
        public TaskController(TaskRepository taskRepository)
        {
            _taskRepository = taskRepository;
        }
        
        [HttpGet]
        public async Task<ActionResult<TaskDto>> GetTasks([FromHeader(Name = "Authorization")] string authToken)
        {
            var username = JwtUtils.GetClaim(authToken, "username");
            var tasks = await _taskRepository.FindAll(task => task.User.Username == username);
            var dtos = tasks.Select(t => new TaskDto(t));
            return Ok(dtos);
        }
        
        [HttpGet("{id}")]
        public async Task<ActionResult<TaskDto>> GetTask([FromRoute] int id)
        {
            var task = await _taskRepository.GetById(id);

            if (task == null)
            {
                return NotFound("Task not found");
            }

            return Ok(new TaskDto(task));
        }
        
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTask([FromRoute] int id,[FromBody] Models.Task task)
        {
            if (id != task.TaskId)
            {
                return BadRequest();
            }

            
            try
            {
                await _taskRepository.Update(task);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _taskRepository.Exists(e => e.TaskId == id))
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
        public async Task<ActionResult<TaskDto>> PostTask([FromBody] Models.Task task)
        {
            var createdTask = await _taskRepository.Create(task);
            var taskDto = new TaskDto(createdTask);
            return CreatedAtAction("GetTask", new { id = taskDto.UserId }, taskDto);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTask(int id)
        {
            var task = await _taskRepository.GetById(id);

            if (task == null)
            {
                return NotFound("Task not found");
            }

            await _taskRepository.Delete(task);

            return NoContent();
        }
    }
}
