namespace DevDoListServer.Models;

public class RoleDto
{
    public RoleDto(Role role)
    {
        RoleId = role.RoleId;
        RoleType = role.RoleType;
    }

    public int RoleId { get; set; }

    public string RoleType { get; set; }
}
