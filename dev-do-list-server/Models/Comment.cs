namespace DevDoListServer.Models;

public partial class Comment
{
    public int CommentId { get; set; }

    public int TaskId { get; set; }

    public string Comment1 { get; set; } = null!;

    public DateTime DateCommented { get; set; }

    public virtual Task Task { get; set; } = null!;
}
