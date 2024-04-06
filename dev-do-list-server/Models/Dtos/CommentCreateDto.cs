namespace DevDoListServer.Models.Dtos;

public class CommentCreateDto
{
    public int TaskId { get; set; }

    public string Comment { get; set; }

    public DateTime DateCommented { get; set; }

    public Comment ToComment()
    {
        return new Comment
        {
            TaskId = TaskId,
            Comment1 = Comment,
            DateCommented = DateCommented
        };
    }
}
