using DevDoListServer.Data;
using DevDoListServer.Models;

namespace DevDoListServer.Repositories
{
    public class UserRepository(AppDbContext context) : GenericRepository<User>(context)
    {
        public async Task<User?> FindByUsername(string username)
        {
            return await base.FindSingle(u => u.Username == username);
        }
    }
}
