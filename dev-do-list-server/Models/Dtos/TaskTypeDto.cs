namespace DevDoListServer.Models.Dtos;

public class TaskTypeDto
{
    public TaskTypeDto()
    {
    }

    public TaskTypeDto(TaskType task)
    {
        TaskTypeId = task.TaskTypeId;
        TaskTypeDescription = task.TaskTypeDescription;
    }

    public int TaskTypeId { get; set; }

    public string TaskTypeDescription { get; set; }

    public TaskType ToTaskType()
    {
        return new TaskType
        {
            TaskTypeId = TaskTypeId,
            TaskTypeDescription = TaskTypeDescription
        };
    }
}
