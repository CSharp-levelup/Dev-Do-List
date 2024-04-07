namespace DevDoListServer.Models.Dtos;

public class TaskResponseDto
{
    public TaskResponseDto(Task task)
    {
        this.TaskId = task.TaskId;
        this.Title = task.Title;
        this.Description = task.Description;
        this.DateCreated = task.DateCreated;
        this.DueDate = task.DueDate;
        this.UserId = task.UserId;
        this.StatusId = task.StatusId;
        this.TaskTypeId = task.TaskTypeId;
        Comments = task.Comments.Select(comment => new CommentDto(comment)).ToList();
    }
    
    public int TaskId { get; set; }

    public string Title { get; set; }

    public string? Description { get; set; }

    public DateTime DateCreated { get; set; }

    public DateTime? DueDate { get; set; }

    public int UserId { get; set; }

    public int StatusId { get; set; }

    public int TaskTypeId { get; set; }

    public ICollection<CommentDto> Comments { get; set; }
}
