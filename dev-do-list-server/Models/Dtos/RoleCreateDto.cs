namespace DevDoListServer.Models.Dtos;

public class RoleCreateDto
{
    public string RoleType { get; set; }

    public Role ToRole()
    {
        return new Role
        {
            RoleType = RoleType
        };
    }
}
