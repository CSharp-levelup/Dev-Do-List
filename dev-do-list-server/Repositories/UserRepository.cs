using DevDoListServer.Data;
using DevDoListServer.Models;
using Microsoft.EntityFrameworkCore;

namespace DevDoListServer.Repositories
{
    public class UserRepository : GenericRepository<User>
    {
        public UserRepository(AppDbContext context) : base(context) { }

        public bool UserExists(int id)
        {
            return context.Users.Any(e => e.UserId == id);
        }

    }
}
