using DevDoListServer.Data;
using DevDoListServer.Models;

namespace DevDoListServer.Repositories
{
    public class TaskTypeRepository: GenericRepository<TaskType>
    {
        public TaskTypeRepository(AppDbContext context) : base(context) { }
    }
}
