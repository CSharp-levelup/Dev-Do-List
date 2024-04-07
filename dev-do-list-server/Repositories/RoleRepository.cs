using DevDoListServer.Data;
using DevDoListServer.Models;

namespace DevDoListServer.Repositories
{
    public class RoleRepository(AppDbContext context) : GenericRepository<Role>(context)
    {
        public async Task<Role?> FindByRoleType(string role)
        {
            return await base.FindSingle(r => r.RoleType == role);
        }
    }
}
