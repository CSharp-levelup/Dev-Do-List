using DevDoListServer.Data;
using DevDoListServer.Models;

namespace DevDoListServer.Repositories;

public class CommentRepository: GenericRepository<Comment>
{
    public CommentRepository(AppDbContext context) : base(context) { }

    public async override Task<Comment?> GetById(int id)
    {
        var comment = await base.GetById(id);
        if (comment != null)
        {
            await context.Entry(comment).Reference(c => c.Task).LoadAsync();
            await context.Entry(comment.Task).Reference(t => t.User).LoadAsync();
        }

        return comment;
    }
}
