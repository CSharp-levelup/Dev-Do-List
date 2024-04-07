using System.Text;
using System.Text.Json;
using dev_do_list_cli.Models;

namespace dev_do_list_cli.Services
{
    public class TaskService
    {
        public async System.Threading.Tasks.Task Create()
        {
            Models.Task task = new();

            using HttpClient client = new();
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", LoginService.JwtToken);

            DateTime dateCreated = DateTime.Now;

            // Fetch task type options
            var taskTypeRequest = new HttpRequestMessage(HttpMethod.Get, "http://dev-do-list-backend.eu-west-1.elasticbeanstalk.com/api/v1/tasktype");
            var taskTypeResponse = await client.SendAsync(taskTypeRequest);
            List<TaskType> taskTypes = new();

            if (taskTypeResponse.IsSuccessStatusCode)
            {
                var taskTypeResponseString = await taskTypeResponse.Content.ReadAsStringAsync();
                taskTypes = JsonSerializer.Deserialize<List<TaskType>>(taskTypeResponseString) ?? [];
                if (taskTypes.Count == 0)
                {
                    Console.WriteLine("Error: There are no valid task types currently available to assign to a task");
                    return;
                }
            }

            // Let the user choose a task type
            Console.WriteLine("Type Options:");
            for (int i = 0; i < taskTypes.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {taskTypes[i].taskTypeDescription}");
            }
            Console.Write("Enter your choice: ");
            string? taskType = Console.ReadLine()?.Trim();

            try
            {
                int taskTypeIndex = int.Parse(taskType);
                task.taskTypeId = taskTypes[taskTypeIndex].taskTypeId;
            }
            catch (Exception)
            {
                Console.WriteLine($"Error: '{taskType}' is not a valid selection.");
                return;
            }


            // Title
            Console.Write("Title: ");
            task.title = Console.ReadLine()?.Trim();
            if (string.IsNullOrEmpty(task.title))
            {
                Console.WriteLine("Error: The title cannot be empty.");
                return;
            }

            // Description
            Console.Write("Description: ");
            task.description = Console.ReadLine()?.Trim();

            // Due date
            Console.Write("Due date (dd/mm/yyyy hh:mm:ss): ");
            string? dueDateString = Console.ReadLine()?.Trim();
            task.dueDate = DateTime.Now; 
            try
            {
                if (!string.IsNullOrEmpty(dueDateString))
                {
                    task.dueDate = DateTime.Parse(dueDateString);
                    if (task.dueDate < dateCreated)
                    {
                        Console.WriteLine("Error: The due date cannot be in the past.");
                        return;
                    }
                }
            }
            catch (FormatException)
            {
                Console.WriteLine("Error: The due date provided is invalid");
                return;
            }

            // Fetch statuses
            var statusRequest = new HttpRequestMessage(HttpMethod.Get, "http://dev-do-list-backend.eu-west-1.elasticbeanstalk.com/api/v1/status");
            var statusResponse = await client.SendAsync(statusRequest);
            List<Status> statuses = [];

            if (statusResponse.IsSuccessStatusCode)
            {
                var statusResponseString = await statusResponse.Content.ReadAsStringAsync();
                statuses = JsonSerializer.Deserialize<List<Status>>(statusResponseString) ?? [];
            }

            task.statusId = statuses.FirstOrDefault(s => s.statusType == "Not Started")?.statusId ?? -1;
            if (task.statusId == -1)
            {
                Console.WriteLine("Error: Couldn't assign a status to the task.");
                return;
            }

            try
            {
                var request = new HttpRequestMessage(HttpMethod.Post, "http://dev-do-list-backend.eu-west-1.elasticbeanstalk.com/api/v1/task");
                request.Content = new StringContent(JsonSerializer.Serialize(task), Encoding.UTF8, "application/json");
                //request.Content = new FormUrlEncodedContent(new Dictionary<string, string>
                //{
                //    { "Title", title },
                //    { "Description", string.IsNullOrEmpty(description) ? "" : description },
                //    { "DateCreated", dateCreated.ToString() },
                //    { "DueDate", dueDate.ToString() },
                //    { "UserId", $"{UserService.UserId}" },
                //    { "StatusId", $"{statusId}" },
                //    { "TaskTypeId", $"{taskTypeId}" }

                //});
                var response = await client.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine("Succesfully created task!");
                }
                else
                {
                    Console.WriteLine("Error: Something went wrong creating the task.");
                }
            }
            catch(Exception)
            {
                Console.WriteLine("Error: Something went wrong creating the task.");
            }

            return;
        }
    }
}
