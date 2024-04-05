using DevDoListServer.Models;
using DevDoListServer.Repositories;

namespace DevDoListServer.Services
{
    public class AuthService
    {
        private readonly UserRepository _userRepository;
        private readonly RoleRepository _roleRepository;
        public AuthService(UserRepository userRepository, RoleRepository roleRepository)
        {
            _userRepository = userRepository;
            _roleRepository = roleRepository;
        }

        public async Task<string> AuthenticateUser(GithubUser githubUser)
        {
            var user = _userRepository.FindByUserName(githubUser.login);

            if (user == null)
            {
                var role = _roleRepository.FindByRoleType(RoleType.User);
                var newUser = new User()
                {
                    Username = githubUser.login,
                    UserPicUrl = githubUser.avatar_url,
                    RoleId = role.RoleId
                };

                await _userRepository.Create(newUser);
                return role.RoleType.ToString();
            }
            var userRole = await _roleRepository.GetById(user.RoleId);
            return userRole is null ? RoleType.User : userRole.RoleType.ToString();
        }
    }
}
