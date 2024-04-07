namespace DevDoListServer.Models.Dtos;

public class CommentDto
{
    public CommentDto()
    {
    }
    public CommentDto(Comment comment)
    {
        CommentId = comment.CommentId;
        TaskId = comment.TaskId;
        Comment = comment.Comment1;
        DateCommented = comment.DateCommented;
    }

    public int CommentId { get; set; }

    public int TaskId { get; set; }

    public string Comment { get; set; }

    public DateTime DateCommented { get; set; }

    public Comment ToComment()
    {
        return new Comment
        {
            CommentId = CommentId,
            TaskId = TaskId,
            Comment1 = Comment,
            DateCommented = DateCommented
        };
    }
}
