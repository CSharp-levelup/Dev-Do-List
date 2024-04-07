namespace DevDoListServer.Models.Dtos;

public class UserCreateDto
{
    public string Username { get; set; }

    public string UserPicUrl { get; set; }

    public int RoleId { get; set; }

    public User ToUser()
    {
        return new User
        {
            Username = Username,
            UserPicUrl = UserPicUrl,
            RoleId = RoleId
        };
    }
}
