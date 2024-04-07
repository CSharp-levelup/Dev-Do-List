using DevDoListServer.Data;
using DevDoListServer.Models;

namespace DevDoListServer.Repositories
{
    public class UserRepository(AppDbContext context) : GenericRepository<User>(context)
    {
        public User? FindByUserName(string username)
        {
           return context.Users.FirstOrDefault(u => u.Username == username);
        }
    }
}
