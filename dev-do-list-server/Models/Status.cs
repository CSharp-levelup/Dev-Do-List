namespace DevDoListServer.Models;

public partial class Status
{
    public int StatusId { get; set; }

    public string StatusType { get; set; } = null!;

    public virtual ICollection<Task> Tasks { get; set; } = new List<Task>();
}
