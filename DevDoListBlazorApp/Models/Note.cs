using System.Text.Json;

namespace DevDoListBlazorApp.Models;

public class Note
{
    public int noteId { get; set; } = -1;
    public string title { get; set; } = "";
    public string description { get; set; } = "";
    public DateTime dueDate {  get; set; } = new DateTime();
    public int userId { get; set; } = -1;
    public int statusId { get; set; } = -1;
    public int taskTypeId { get; set; } = -1;
    public Comment[] comments { get; set; } = [];
}