namespace DevDoListServer.Models.Dtos;

public class UserDto
{
    public UserDto()
    {
    }

    public UserDto(User user)
    {
        UserId = user.UserId;
        Username = user.Username;
        UserPicUrl = user.UserPicUrl;
        RoleId = user.RoleId;
    }

    public int UserId { get; set; }

    public string Username { get; set; }

    public string UserPicUrl { get; set; }

    public int RoleId { get; set; }

    public User ToUser()
    {
        return new User
        {
            UserId = UserId,
            Username = Username,
            UserPicUrl = UserPicUrl,
            RoleId = RoleId
        };
    }
}
