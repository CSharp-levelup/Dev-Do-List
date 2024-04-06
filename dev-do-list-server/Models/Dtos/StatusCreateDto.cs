namespace DevDoListServer.Models.Dtos;

public class StatusCreateDto
{
    public string StatusType { get; set; }

    public Status ToStatus()
    {
        return new Status
        {
            StatusType = StatusType
        };
    }
}
