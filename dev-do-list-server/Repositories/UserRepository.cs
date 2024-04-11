using DevDoListServer.Data;
using DevDoListServer.Models;
using Microsoft.EntityFrameworkCore;

namespace DevDoListServer.Repositories
{
    public class UserRepository(AppDbContext context) : GenericRepository<User>(context)
    {
        public async Task<User?> FindByUsername(string username)
        {
            return await context.Users.FirstOrDefaultAsync(u => u.Username == username);
        }
    }
}
