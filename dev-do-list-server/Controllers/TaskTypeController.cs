using DevDoListServer.Models.Dtos;
using DevDoListServer.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
    public async Task<ActionResult<IEnumerable<TaskTypeDto>>> GetTaskTypes()
    {
        var taskTypes = await _taskTypeRepository.GetAll();
        var dtos = taskTypes.Select(taskType => new TaskTypeDto(taskType)).ToList();
        return Ok(dtos);
    }
    
    [HttpGet("{id}")]
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
    public async Task<ActionResult<TaskTypeDto>> PostTaskType([FromBody] TaskTypeCreateDto taskType)
    {
        var createdTaskType = await _taskTypeRepository.Create(taskType.ToTaskType());
        var taskTypeDto = new TaskTypeDto(createdTaskType);
        return CreatedAtAction("GetTaskType", new { id = taskTypeDto.TaskTypeId }, taskTypeDto);
    }
    
    [HttpDelete("{id}")]
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
