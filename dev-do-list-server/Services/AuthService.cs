using DevDoListServer.Models;
using DevDoListServer.Repositories;

namespace DevDoListServer.Services
{
    public class AuthService(UserRepository userRepository, RoleRepository roleRepository)
    {
        public async Task<string> AuthenticateUser(GithubUser githubUser)
        {
            var user = await userRepository.FindByUsername(githubUser.login);

            if (user is null)
            {
                var role = await roleRepository.FindByRoleType(RoleType.User);
                var newUser = new User()
                {
                    Username = githubUser.login,
                    UserPicUrl = githubUser.avatar_url,
                    RoleId = role!.RoleId
                };

                await userRepository.Create(newUser);
                return role.RoleType;
            }

            var userRole = await roleRepository.GetById(user.RoleId);
            return userRole is null ? RoleType.User : userRole.RoleType;
        }
    }
}
