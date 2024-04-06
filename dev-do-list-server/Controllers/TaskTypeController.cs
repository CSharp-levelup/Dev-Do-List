using DevDoListServer.Models;
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
    public async Task<ActionResult<IEnumerable<TaskType>>> GetTaskTypes()
    {
        return await _taskTypeRepository.GetAll();
    }
    
    [HttpGet("{id}")]
    public async Task<ActionResult<TaskType>> GetTaskType([FromRoute] int id)
    {
        var taskType = await _taskTypeRepository.GetById(id);

        if (taskType == null)
        {
            return NotFound("Task type not found");
        }

        return Ok(taskType);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> PutTaskType([FromRoute] int id, [FromBody] TaskType taskType)
    {
        if (id != taskType.TaskTypeId)
        {
            return BadRequest();
        }

            
        try
        {
            await _taskTypeRepository.Update(taskType);
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
    public async Task<ActionResult<TaskType>> PostTaskType([FromBody] TaskType taskType)
    {
        var createdTaskType = await _taskTypeRepository.Create(taskType);

        return CreatedAtAction("GetTaskType", new { id = createdTaskType.TaskTypeId }, createdTaskType);
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
