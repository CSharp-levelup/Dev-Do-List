namespace DevDoListServer.Models.Dtos;

public class TaskTypeCreateDto
{
    public string TaskTypeDescription { get; set; }

    public TaskType ToTaskType()
    {
        return new TaskType
        {
            TaskTypeDescription = TaskTypeDescription
        };
    }
}
