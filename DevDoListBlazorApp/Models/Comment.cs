namespace DevDoListBlazorApp.Models;

public class Comment

{
    public int commentId { get; set; }
    public int taskId { get; set; }
    public string comment { get; set; }
    public DateTime dateCommented { get; set; }
}