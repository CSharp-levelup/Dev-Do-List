namespace DevDoListServer.Models.Dtos;

public class TaskCreateDto
{
    public string Title { get; set; }

    public string? Description { get; set; }

    public DateTime DateCreated { get; set; }

    public DateTime? DueDate { get; set; }

    public int UserId { get; set; }

    public int StatusId { get; set; }

    public int TaskTypeId { get; set; }

    public Task ToTask()
    {
        return new Task
        {
            Title = Title,
            Description = Description,
            DueDate = DueDate,
            UserId = UserId,
            StatusId = StatusId,
            TaskTypeId = TaskTypeId
        };
    }
}
