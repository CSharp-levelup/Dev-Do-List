
namespace DevDoListBlazorApp.Models;

public class Comment

{
    public int commentId { get; set; } = -1;

    public int taskId { get; set; } = -1;
    public string comment { get; set; } = "";
    public DateTime dateCommented { get; set; } = new DateTime();
}