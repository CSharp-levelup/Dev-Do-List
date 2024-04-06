namespace DevDoListServer.Models.Dtos;

public class RoleResponseDto
{
    public RoleResponseDto(Role role)
    {
        RoleId = role.RoleId;
        RoleType = role.RoleType;
    }

    public int RoleId { get; set; }

    public string RoleType { get; set; }
}
