namespace DevDoListServer.Models.Dtos;

public class TaskUpdateDTO
{
    public int TaskId { get; set; }

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
            TaskId = TaskId,
            Title = Title,
            Description = Description,
            DateCreated = DateCreated,
            DueDate = DueDate,
            UserId = UserId,
            StatusId = StatusId,
            TaskTypeId = TaskTypeId
        };
    }
}
