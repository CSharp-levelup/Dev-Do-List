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
    public class TaskController(TaskRepository taskRepository, UserRepository userRepository) : ControllerBase
    {
        [HttpGet]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(IEnumerable<TaskResponseDto>))]
        [SwaggerResponse(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<TaskResponseDto>> GetTasks([FromHeader(Name = "Authorization")] string authToken)
        {
            var username = JwtUtils.GetClaim(authToken, "username");
            var tasks = await taskRepository.FindAll(task => task.User!.Username == username);
            var dtos = tasks.Select(t => new TaskResponseDto(t));
            return Ok(dtos);
        }
        
        [HttpGet("{id}")]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(TaskResponseDto))]
        [SwaggerResponse(StatusCodes.Status401Unauthorized)]
        [SwaggerResponse(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<TaskResponseDto>> GetTask([FromHeader(Name = "Authorization")] string authToken, [FromRoute] int id)
        {
            var username = JwtUtils.GetClaim(authToken, "username");
            var task = await taskRepository.GetById(id);
            
            if (task is null)
            {
                return NotFound("Task not found");
            }

            if (task.User!.Username != username)
            {
                return Unauthorized();
            }

            return Ok(new TaskResponseDto(task));
        }
        
        [HttpPut("{id}")]
        [SwaggerResponse(StatusCodes.Status204NoContent)]
        [SwaggerResponse(StatusCodes.Status401Unauthorized)]
        [SwaggerResponse(StatusCodes.Status404NotFound)]
        [SwaggerResponse(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> PutTask([FromHeader(Name = "Authorization")] string authToken,
            [FromRoute] int id, [FromBody] TaskUpdateDTO taskUpdateDto)
        {
            var username = JwtUtils.GetClaim(authToken, "username");

            if (id != taskUpdateDto.TaskId)
            {
                return BadRequest();
            }

            var task = await taskRepository.GetById(taskUpdateDto.TaskId);
            
            
            if (task is null)
            {
                return NotFound("Task not found");
            }

            if (task!.User!.Username != username)
            {
                return Unauthorized();
            }

            if (taskUpdateDto.UserId != task.UserId)
            {
                return BadRequest("User ID cannot be changed");
            }
            
            await taskRepository.Update(taskUpdateDto.ToTask());
            

            return NoContent();
        }

        [HttpPost]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(TaskResponseDto))]
        public async Task<ActionResult<TaskResponseDto>> PostTask([FromHeader(Name = "Authorization")] string authToken, [FromBody] TaskCreateDto taskCreateDto)
        {
            var username = JwtUtils.GetClaim(authToken, "username");
            var user = await userRepository.FindSingle(u => u.Username == username);
            taskCreateDto.UserId = user.UserId;
            var createdTask = await taskRepository.Create(taskCreateDto.ToTask());
            var taskDto = new TaskResponseDto(createdTask);
            return CreatedAtAction("GetTask", new { id = taskDto.UserId }, taskDto);
        }

        [HttpDelete("{id}")]
        [SwaggerResponse(StatusCodes.Status204NoContent)]
        [SwaggerResponse(StatusCodes.Status401Unauthorized)]
        [SwaggerResponse(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteTask([FromHeader(Name = "Authorization")] string authToken, int id)
        {
            var username = JwtUtils.GetClaim(authToken, "username");

            var task = await taskRepository.GetById(id);

            if (task is null)
            {
                return NotFound("Task not found");
            }

            if (task!.User!.Username != username)
            {
                return Unauthorized();
            }

            await taskRepository.Delete(task);

            return NoContent();
        }
    }
}
