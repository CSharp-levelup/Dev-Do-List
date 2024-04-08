using System.Globalization;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using dev_do_list_cli.Models;

namespace dev_do_list_cli.Services
{
    public class TaskService
    {
        public TaskService() 
        {
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", LoginService.JwtToken);
        }

        public async Task RefreshLocalTasks()
        {
            // Fetch all tasks
            var taskRequest = new HttpRequestMessage(HttpMethod.Get, $"http://dev-do-list-backend.eu-west-1.elasticbeanstalk.com/api/v1/task");
            var taskResponse = await client.SendAsync(taskRequest);

            if (taskResponse.IsSuccessStatusCode)
            {
                var taskTypeResponseString = await taskResponse.Content.ReadAsStringAsync();
                this.tasks = JsonSerializer.Deserialize<List<TaskResponse>>(taskTypeResponseString) ?? [];
            }
            else
            {
                throw new Exception("Failed to get the user's tasks from the API");
            }
        }

        public async Task RefreshLocalStatuses()
        {
            // Fetch all statuses
            var statusRequest = new HttpRequestMessage(HttpMethod.Get, "http://dev-do-list-backend.eu-west-1.elasticbeanstalk.com/api/v1/status");
            var statusResponse = await client.SendAsync(statusRequest);

            if (statusResponse.IsSuccessStatusCode)
            {
                var statusResponseString = await statusResponse.Content.ReadAsStringAsync();
                this.statuses = JsonSerializer.Deserialize<List<StatusResponse>>(statusResponseString) ?? [];
            }
            else
            {
                throw new Exception("Failed to get the statuses from the API");
            }
        }

        public async Task RefreshLocalTypes()
        {
            // Fetch all types
            var taskTypeRequest = new HttpRequestMessage(HttpMethod.Get, "http://dev-do-list-backend.eu-west-1.elasticbeanstalk.com/api/v1/tasktype");
            var taskTypeResponse = await client.SendAsync(taskTypeRequest);

            if (taskTypeResponse.IsSuccessStatusCode)
            {
                var taskTypeResponseString = await taskTypeResponse.Content.ReadAsStringAsync();
                this.types = JsonSerializer.Deserialize<List<TaskTypeResponse>>(taskTypeResponseString) ?? [];
            }
            else
            {
                throw new Exception("Failed to get the types from the API");
            }
        }

        public async Task Create()
        {
            TaskCreate task = new();

            task.userId = UserService.UserId;

            var currentDate = DateTime.Now;
            task.dateCreated = new DateTime(
                currentDate.Year,
                currentDate.Month,
                currentDate.Day,
                currentDate.Hour,
                currentDate.Minute,
                0
            );

            // Fetch task type options
            await this.RefreshLocalTypes();

            if (this.types.Count == 0)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Error: There are no valid task types currently available to assign to a task");
                Console.ResetColor();
                return;
            }

            // Let the user choose a task type
            Console.WriteLine("Type Options:");
            for (int i = 0; i < this.types.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {this.types[i].taskTypeDescription}");
            }
            Console.Write("Enter your choice: ");
            string? taskType = Console.ReadLine()?.Trim();

            try
            {
                int taskTypeIndex = int.Parse(taskType) - 1;
                if (taskTypeIndex < 0)
                {
                    throw new Exception("Index cannot be less than 0");
                }
                task.taskTypeId = this.types[taskTypeIndex].taskTypeId;
            }
            catch (Exception)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Error: '{taskType}' is not a valid selection.");
                Console.ResetColor();
                return;
            }

            // Title
            Console.Write("Title: ");
            task.title = Console.ReadLine()?.Trim();
            if (string.IsNullOrEmpty(task.title))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Error: The title cannot be empty.");
                Console.ResetColor();
                return;
            }

            // Description
            Console.Write("Description: ");
            task.description = Console.ReadLine()?.Trim();

            // Due date
            Console.Write($"Due date ({format}): ");
            string? dueDateString = Console.ReadLine()?.Trim();
            task.dueDate = task.dateCreated; 
            try
            {
                if (!string.IsNullOrEmpty(dueDateString))
                {
                    task.dueDate = DateTime.ParseExact(dueDateString, format, culture, DateTimeStyles.None);
                    if (task.dueDate < task.dateCreated)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Error: The due date cannot be in the past.");
                        Console.ResetColor();
                        return;
                    }
                }
            }
            catch (FormatException)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Error: The due date provided is invalid");
                Console.ResetColor();
                return;
            }

            // Fetch statuses
            await this.RefreshLocalStatuses();

            task.statusId = this.statuses.FirstOrDefault(s => s.statusType.ToLower() == "not started")?.statusId ?? -1;
            if (task.statusId == -1)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Error: Couldn't assign a status to the task.");
                Console.ResetColor();
                return;
            }

            try
            {
                var json = JsonSerializer.Serialize(task);
                var request = new HttpRequestMessage(HttpMethod.Post, "http://dev-do-list-backend.eu-west-1.elasticbeanstalk.com/api/v1/task");
                request.Content = new StringContent(JsonSerializer.Serialize(task), Encoding.UTF8, "application/json");
                var response = await client.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine("Succesfully created the task!");
                }
                else
                {
                    Console.WriteLine("Error: Something went wrong creating the task.", ConsoleColor.Red);
                }
            }
            catch(Exception)
            {
                Console.WriteLine("Error: Something went wrong creating the task.", ConsoleColor.Red);
            }

            await this.RefreshLocalTasks();
        }
    
        public async Task List()
        {
            try { 
                if (this.tasks.Count == 0)
                {
                    Console.WriteLine("Your to do list is empty");
                    return;
                }

                // Fetch statuses
                await this.RefreshLocalStatuses();

                Console.WriteLine($"{"ID",-5}{"Task",-35}{"Status",-10}");
                Console.WriteLine("--------------------------------------------------");
                for (int i = 0; i < tasks.Count; i++)
                {
                    Console.WriteLine($"{$"{i + 1}.", -5}{tasks[i].title, -35}{this.statuses.First(s => s.statusId == tasks[i].statusId).statusType, -10}");
                }
                
            }
            catch(Exception)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Error: Unable to fetch your tasks at the moment.");
                return;
            }
        }
    
        public void Details(string listIdString)
        {
            int listId; 
            var task = new TaskResponse();
            try
            {
                listId = int.Parse(listIdString);
                task = this.tasks[listId - 1];
            }
            catch (Exception)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Error: '{listIdString}' is not a valid option");
                Console.ResetColor();
                return;
            }
            Console.WriteLine($"Task ID: {listId}");
            Console.WriteLine($"Title: {task.title}");
            Console.WriteLine($"Status: {this.statuses.First(s => s.statusId == task.statusId).statusType}");
            Console.WriteLine($"Type: {this.types.First(t => t.taskTypeId == task.taskTypeId).taskTypeDescription}");
            Console.WriteLine($"Description: {(string.IsNullOrEmpty(task.description) ? "None" : task.description)}");
            Console.WriteLine($"Due Date: {task.dueDate}");
            Console.WriteLine($"Date Created: {task.dateCreated}");
            Console.Write($"Comments:{(task.comments.Count == 0 ? " None" : "")}");
            foreach(var comment in task.comments)
            {
                Console.Write($"> \"{comment.comment}\"");
            }
            Console.WriteLine();
        }

        public async Task Delete(string listIdString)
        {
            int listId;
            var task = new TaskResponse();
            try
            {
                listId = int.Parse(listIdString);
                task = this.tasks[listId - 1];
            }
            catch (Exception)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Error: '{listIdString}' is not a valid option");
                Console.ResetColor();
                return;
            }

            try
            {
                var request = new HttpRequestMessage(HttpMethod.Delete, $"http://dev-do-list-backend.eu-west-1.elasticbeanstalk.com/api/v1/task/{task.taskId}");
                var response = await client.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"Task {listId} was deleted succesfully!");
                }
                else
                {
                    throw new Exception("Unable to delete task");
                }
            }
            catch (Exception)
            {
                Console.ForegroundColor= ConsoleColor.Red;
                Console.WriteLine("Error: the task could not be deleted");
                Console.ResetColor();
            }

            await this.RefreshLocalTasks();
        }

        public async Task Update(string listIdString)
        {
            int listId;
            var task = new TaskResponse();
            try
            {
                listId = int.Parse(listIdString);
                task = this.tasks[listId - 1];
            }
            catch (Exception)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Error: '{listIdString}' is not a valid option");
                Console.ResetColor();
                return;
            }

            Console.Write("Would you like to update the task's type (y/n): ");
            string? typeInput = Console.ReadLine()?.Trim();

            var type = types.First(t => t.taskTypeId == task.taskTypeId);

            switch (typeInput)
            {
                case "y":
                    Console.WriteLine("Choose a task type to update to:");
                    for (int i = 0; i < types.Count; i++) 
                    {
                        Console.WriteLine($"{i + 1}. {types[i].taskTypeDescription}");
                    }
                    Console.Write("Enter your choice: ");
                    string? typeChoice = Console.ReadLine()?.Trim();
                    try
                    {
                        int typeChoiceIndex = int.Parse(typeChoice);
                        type = types[typeChoiceIndex - 1];
                    }
                    catch(Exception)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($"Error: '{typeChoice}' is not a valid choice.");
                        Console.ResetColor();
                        return;
                    }
                    break;
                case "n":
                    break;
                default:
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"Error: '{typeInput}' is not a valid choice.");
                    Console.ResetColor();
                    return;
            }

            Console.Write("Would you like to update the task's status (y/n): ");
            string? statusInput = Console.ReadLine()?.Trim();

            var status = statuses.First(s => s.statusId == task.statusId);

            switch (statusInput)
            {
                case "y":
                    Console.WriteLine("Choose a status to update to:");
                    for (int i = 0; i < statuses.Count; i++)
                    {
                        Console.WriteLine($"{i + 1}. {statuses[i].statusType}");
                    }
                    Console.Write("Enter your choice: ");
                    string? statusChoice = Console.ReadLine()?.Trim();
                    try
                    {
                        int statusChoiceIndex = int.Parse(statusChoice);
                        status = statuses[statusChoiceIndex - 1];
                    }
                    catch (Exception)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($"Error: '{statusChoice}' is not a valid choice.");
                        Console.ResetColor();
                        return;
                    }
                    break;
                case "n":
                    break;
                default:
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"Error: '{statusInput}' is not a valid choice.");
                    Console.ResetColor();
                    return;
            }

            Console.Write("Would you like to update the task's due date (y/n): ");
            string? dueDateInput = Console.ReadLine()?.Trim();

            var dueDate = task.dueDate;

            switch (dueDateInput)
            {
                case "y":
                    Console.Write($"Due date ({format}): ");
                    string? dueDateString = Console.ReadLine()?.Trim();
                    try
                    {
                        if (!string.IsNullOrEmpty(dueDateString))
                        {
                            dueDate = DateTime.ParseExact(dueDateString, format, culture, DateTimeStyles.None);
                            if (dueDate < task.dateCreated)
                            {
                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.WriteLine("Error: The due date cannot be before the task was created.");
                                Console.ResetColor();
                                return;
                            }
                        }
                    }
                    catch (FormatException)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Error: The due date provided is invalid");
                        Console.ResetColor();
                        return;
                    }
                    break;
                case "n":
                    break;
                default:
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"Error: '{dueDateInput}' is not a valid choice.");
                    Console.ResetColor();
                    return;
            }

            task.taskTypeId = type.taskTypeId;
            task.statusId = status.statusId;
            task.dueDate = dueDate;

            try
            {
                var request = new HttpRequestMessage(HttpMethod.Put, $"http://dev-do-list-backend.eu-west-1.elasticbeanstalk.com/api/v1/task/{task.taskId}");
                request.Content = new StringContent(JsonSerializer.Serialize(task), Encoding.UTF8, "application/json");
                var response = await client.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine("Succesfully updated the task!");
                }
                else
                {
                    throw new Exception("Failed to update the task");
                }
            }
            catch
            {
                Console.ForegroundColor= ConsoleColor.Red;
                Console.WriteLine("Error: Failed to update the task");
                Console.ResetColor();
                return;
            }

            await this.RefreshLocalTasks();
        }

        private HttpClient client = new HttpClient();

        private string format = "dd/MM/yyyy HH:mm";
            
        private CultureInfo culture = new CultureInfo("en-GB");

        private List<TaskResponse> tasks = new List<TaskResponse>();

        private List<StatusResponse> statuses = new List<StatusResponse>();

        private List<TaskTypeResponse> types = new List<TaskTypeResponse>();
    }
}
