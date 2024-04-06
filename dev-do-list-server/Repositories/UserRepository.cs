using DevDoListServer.Data;
using DevDoListServer.Models;

namespace DevDoListServer.Repositories
{
    public class UserRepository : GenericRepository<User>
    {
        public UserRepository(AppDbContext context) : base(context) { }

        public User? FindByUserName(string username)
        {
           return context.Users.Where(u => u.Username == username).FirstOrDefault();
        }
    }
}
