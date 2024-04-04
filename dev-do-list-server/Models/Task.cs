namespace DevDoListServer.Models;

public partial class Task
{
    public int TaskId { get; set; }

    public string Title { get; set; } = null!;

    public string? Description { get; set; }

    public DateTime DateCreated { get; set; }

    public DateTime? DueDate { get; set; }

    public int UserId { get; set; }

    public int StatusId { get; set; }

    public int TaskTypeId { get; set; }

    public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();

    public virtual Status Status { get; set; } = null!;

    public virtual TaskType TaskType { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
