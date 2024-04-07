using DevDoListServer.Models.Dtos;
using DevDoListServer.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.Annotations;

namespace DevDoListServer.Controllers;

[Route("api/v1/tasktype")]
[ApiController]
public class TaskTypeController : ControllerBase
{
    private readonly TaskTypeRepository _taskTypeRepository;

    public TaskTypeController(TaskTypeRepository taskTypeRepository)
    {
        _taskTypeRepository = taskTypeRepository;
    }
    
    [HttpGet]
    [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(IEnumerable<TaskTypeDto>))]
    [SwaggerResponse(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<IEnumerable<TaskTypeDto>>> GetTaskTypes()
    {
        var taskTypes = await _taskTypeRepository.GetAll();
        var dtos = taskTypes.Select(taskType => new TaskTypeDto(taskType)).ToList();
        return Ok(dtos);
    }
    
    [HttpGet("{id}")]
    [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(TaskTypeDto))]
    [SwaggerResponse(StatusCodes.Status401Unauthorized)]
    [SwaggerResponse(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TaskTypeDto>> GetTaskType([FromRoute] int id)
    {
        var taskType = await _taskTypeRepository.GetById(id);

        if (taskType == null)
        {
            return NotFound("Task type not found");
        }

        return Ok(new TaskTypeDto(taskType));
    }

    [HttpPut("{id}")]
    [SwaggerResponse(StatusCodes.Status204NoContent)]
    [SwaggerResponse(StatusCodes.Status401Unauthorized)]
    [SwaggerResponse(StatusCodes.Status404NotFound)]
    [SwaggerResponse(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> PutTaskType([FromRoute] int id, [FromBody] TaskTypeDto taskType)
    {
        if (id != taskType.TaskTypeId)
        {
            return BadRequest();
        }

            
        try
        {
            await _taskTypeRepository.Update(taskType.ToTaskType());
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!await _taskTypeRepository.Exists(e => e.TaskTypeId == id))
            {
                return NotFound("Task type not found");
            }
            else
            {
                throw;
            }
        }

        return NoContent();
    }

    [HttpPost]
    [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(TaskTypeDto))]
    public async Task<ActionResult<TaskTypeDto>> PostTaskType([FromBody] TaskTypeCreateDto taskType)
    {
        var createdTaskType = await _taskTypeRepository.Create(taskType.ToTaskType());
        var taskTypeDto = new TaskTypeDto(createdTaskType);
        return CreatedAtAction("GetTaskType", new { id = taskTypeDto.TaskTypeId }, taskTypeDto);
    }
    
    [HttpDelete("{id}")]
    [SwaggerResponse(StatusCodes.Status204NoContent)]
    [SwaggerResponse(StatusCodes.Status401Unauthorized)]
    [SwaggerResponse(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteTaskType([FromRoute] int id)
    {
        var taskType = await _taskTypeRepository.GetById(id);

        if (taskType == null)
        {
            return NotFound("Task Type not found");
        }

        await _taskTypeRepository.Delete(taskType);

        return NoContent();
    }
}
