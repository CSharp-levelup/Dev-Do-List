namespace DevDoListServer.Models;

public class CommentDto
{
    public CommentDto(Comment comment)
    {
        this.CommentId = comment.CommentId;
        this.TaskId = comment.TaskId;
        this.Comment = comment.Comment1;
        this.DateCommented = comment.DateCommented;
    }
    public int CommentId { get; set; }

    public int TaskId { get; set; }

    public string Comment { get; set; }

    public DateTime DateCommented { get; set; }
}
