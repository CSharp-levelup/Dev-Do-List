using DevDoListServer.Data;
using DevDoListServer.Models;

namespace DevDoListServer.Repositories
{
    public class RoleRepository : GenericRepository<Role>
    {
        public RoleRepository(AppDbContext context) : base(context) { }
    }
}
