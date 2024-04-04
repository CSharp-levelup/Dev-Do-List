using DevDoListServer.Data;
using DevDoListServer.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DevDoListServer.Controllers;

[Route("api/[controller]")]
[ApiController]
public class TaskTypeController : ControllerBase
{
    private readonly AppDbContext _context;

    public TaskTypeController(AppDbContext context)
    {
        _context = context;
    }

    // GET: api/TaskType
    [HttpGet]
    public async Task<ActionResult<IEnumerable<TaskType>>> GetTaskTypes()
    {
        return await _context.TaskTypes.ToListAsync();
    }

    // GET: api/TaskType/5
    [HttpGet("{id}")]
    public async Task<ActionResult<TaskType>> GetTaskType(int id)
    {
        var taskType = await _context.TaskTypes.FindAsync(id);

        if (taskType == null) return NotFound();

        return taskType;
    }

    // PUT: api/TaskType/5
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPut("{id}")]
    public async Task<IActionResult> PutTaskType(int id, TaskType taskType)
    {
        if (id != taskType.TaskTypeId) return BadRequest();

        _context.Entry(taskType).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!TaskTypeExists(id))
                return NotFound();
            throw;
        }

        return NoContent();
    }

    // POST: api/TaskType
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPost]
    public async Task<ActionResult<TaskType>> PostTaskType(TaskType taskType)
    {
        _context.TaskTypes.Add(taskType);
        await _context.SaveChangesAsync();

        return CreatedAtAction("GetTaskType", new { id = taskType.TaskTypeId }, taskType);
    }

    // DELETE: api/TaskType/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTaskType(int id)
    {
        var taskType = await _context.TaskTypes.FindAsync(id);
        if (taskType == null) return NotFound();

        _context.TaskTypes.Remove(taskType);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool TaskTypeExists(int id)
    {
        return _context.TaskTypes.Any(e => e.TaskTypeId == id);
    }
}
