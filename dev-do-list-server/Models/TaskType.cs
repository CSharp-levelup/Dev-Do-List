namespace DevDoListServer.Models;

public partial class TaskType
{
    public int TaskTypeId { get; set; }

    public string TaskTypeDescription { get; set; } = null!;

    public virtual ICollection<Task> Tasks { get; set; } = new List<Task>();
}
