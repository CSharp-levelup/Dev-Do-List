using DevDoListServer.Data;
using DevDoListServer.Models;

namespace DevDoListServer.Repositories
{
    public class StatusRepository(AppDbContext context) : GenericRepository<Status>(context);
}
 
