using DevDoListServer.Data;
using DevDoListServer.Models;

namespace DevDoListServer.Repositories
{
    public class StatusRepository: GenericRepository<Status>
    {
        public StatusRepository(AppDbContext context) : base(context) { }
    }
}
 