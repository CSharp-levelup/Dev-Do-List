using DevDoListServer.Data;
using DevDoListServer.Models;

namespace DevDoListServer.Repositories
{
    public class RoleRepository : GenericRepository<Role>
    {
        public RoleRepository(AppDbContext context) : base(context) { }

        public Role? FindByRoleType(string role)
        {
            return context.Roles.Where(u => u.RoleType == role).FirstOrDefault();
        }
    }
}
