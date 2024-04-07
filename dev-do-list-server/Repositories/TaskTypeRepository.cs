using DevDoListServer.Data;
using DevDoListServer.Models;

namespace DevDoListServer.Repositories
{
    public class TaskTypeRepository(AppDbContext context) : GenericRepository<TaskType>(context);
}
