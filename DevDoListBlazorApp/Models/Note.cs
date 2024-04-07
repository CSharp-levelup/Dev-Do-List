using System.Text.Json;

namespace DevDoListBlazorApp.Models;

public class Note
{
    public int noteId { get; set; }
    public string title { get; set; }
    public string description { get; set; }
    public DateTime dueDate {  get; set; }
    public int userId { get; set; }
    public int statusId { get; set; }
    public int taskTypeId { get; set; }
    public Comment[] comments { get; set; }
}