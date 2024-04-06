using DevDoListServer.Data;
using Microsoft.EntityFrameworkCore;

namespace DevDoListServer.Repositories;

public class TaskRepository: GenericRepository<Models.Task>
{
    public TaskRepository(AppDbContext context) : base(context) { }

    public override async Task<Models.Task?> GetById(int id)
    {
        var task = await base.GetById(id);
        if (task != null)
        {
            await context.Entry(task).Collection(t => t.Comments).LoadAsync();
        }

        return task;
    }
}
