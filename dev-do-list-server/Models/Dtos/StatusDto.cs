namespace DevDoListServer.Models.Dtos;

public class StatusDto
{
    public StatusDto()
    {
    }

    public StatusDto(Status status)
    {
        StatusId = status.StatusId;
        StatusType = status.StatusType;
    }

    public int StatusId { get; set; }

    public string StatusType { get; set; }

    public Status ToStatus()
    {
        return new Status
        {
            StatusId = StatusId,
            StatusType = StatusType
        };
    }
}
