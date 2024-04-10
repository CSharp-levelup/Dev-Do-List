using System.Globalization;
using System.Text;
using System.Text.Json;
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
                tasks = JsonSerializer.Deserialize<List<TaskResponse>>(taskTypeResponseString) ?? [];
            }
            else
            {
                throw new Exception("Failed to get the user's tasks from the API");
            }

            // Fetch comments for tasks
            foreach (var task in tasks)
            {
                var commentRequest = new HttpRequestMessage(HttpMethod.Get, $"http://dev-do-list-backend.eu-west-1.elasticbeanstalk.com/api/v1/comment/task/{task.taskId}");
                var commentResponse = await client.SendAsync(commentRequest);

                if (commentResponse.IsSuccessStatusCode) 
                {
                    var commentReponseString = await commentResponse.Content.ReadAsStringAsync();
                    task.comments = JsonSerializer.Deserialize<List<CommentResponse>>(commentReponseString) ?? [];
                }
                else if (commentResponse.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    task.comments = [];
                }
                else
                {
                    throw new Exception("Failed to get the user's comments from the API");
                }
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
                statuses = JsonSerializer.Deserialize<List<StatusResponse>>(statusResponseString) ?? [];
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
                types = JsonSerializer.Deserialize<List<TaskTypeResponse>>(taskTypeResponseString) ?? [];
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
            await RefreshLocalTypes();

            if (types.Count == 0)
            {
                ConsoleService.Error("There are no valid task types currently available to assign to a task.");
                return;
            }

            // Let the user choose a task type
            Console.WriteLine("Type Options:");
            for (int i = 0; i < types.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {types[i].taskTypeDescription}");
            }
            Console.Write("Enter your choice: ");
            string? taskType = Console.ReadLine()?.Trim();

            try
            {
                int taskTypeIndex = int.Parse(taskType) - 1;
                if (taskTypeIndex < 0)
                {
                    throw new Exception("Index cannot be less than 0.");
                }
                task.taskTypeId = types[taskTypeIndex].taskTypeId;
            }
            catch (Exception)
            {
                ConsoleService.Error($"'{taskType}' is not a valid selection.");
                return;
            }

            // Title
            Console.Write("Title: ");
            task.title = Console.ReadLine()?.Trim() ?? "";
            if (string.IsNullOrEmpty(task.title))
            {
                ConsoleService.Error("The title cannot be empty.");
                return;
            }
            if (task.title.Length > 50)
            {
                ConsoleService.Error("The title cannot be more than 50 characters");
                return;
            }

            // Description
            Console.Write("Description: ");
            task.description = Console.ReadLine()?.Trim() ?? "";
            if (task.description.Length > 100)
            {
                ConsoleService.Error("The description cannot be more than 100 characters");
                return;
            }

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
                        ConsoleService.Error("The due date cannot be in the past.");
                        return;
                    }
                }
            }
            catch (FormatException)
            {
                ConsoleService.Error("The due date provided is invalid.");
                return;
            }

            // Fetch statuses
            await RefreshLocalStatuses();

            task.statusId = statuses.FirstOrDefault(s => s.statusType.ToLower() == "not started")?.statusId ?? -1;
            if (task.statusId == -1)
            {
                ConsoleService.Error("Couldn't assign a status to the task.");
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
                    throw new Exception("Something went wrong creating the task.");
                }
            }
            catch(Exception)
            {
                ConsoleService.Error("Something went wrong creating the task.");
            }

            await RefreshLocalTasks();
        }
    
        public async Task List()
        {
            try { 
                if (tasks.Count == 0)
                {
                    Console.WriteLine("Your to do list is empty.");
                    return;
                }

                // Fetch statuses
                await this.RefreshLocalStatuses();

                Console.WriteLine($"{"ID",-5}{"Task",-35}{"Status",-10}");
                Console.WriteLine("--------------------------------------------------");
                for (int i = 0; i < tasks.Count; i++)
                {
                    Console.WriteLine($"{$"{i + 1}.", -5}{tasks[i].title, -35}{statuses.First(s => s.statusId == tasks[i].statusId).statusType, -10}");
                }
                
            }
            catch(Exception)
            {
                ConsoleService.Error("Unable to fetch your tasks at the moment.");
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
                task = tasks[listId - 1];
            }
            catch (Exception)
            {
                ConsoleService.Error($"'{listIdString}' is not a valid option.");
                return;
            }
            Console.WriteLine($"Task ID: {listId}");
            Console.WriteLine($"Title: {task.title}");
            Console.WriteLine($"Status: {statuses.First(s => s.statusId == task.statusId).statusType}");
            Console.WriteLine($"Type: {types.First(t => t.taskTypeId == task.taskTypeId).taskTypeDescription}");
            Console.WriteLine($"Description: {(string.IsNullOrEmpty(task.description) ? "None" : task.description)}");
            Console.WriteLine($"Due Date: {task.dueDate}");
            Console.WriteLine($"Date Created: {task.dateCreated}");
            Console.Write($"Comments:{(task.comments.Count == 0 ? " None" : "\n")}");
            foreach(var comment in task.comments)
            {
                Console.WriteLine($" > ({comment.dateCommented.ToString(format)}) - \"{comment.comment}\"");
            }
            Console.WriteLine();
        }

        public async Task Delete(string listIdString)
        {
            int listId;
            TaskResponse? task;
            try
            {
                listId = int.Parse(listIdString);
                task = tasks[listId - 1];
            }
            catch (Exception)
            {
                ConsoleService.Error($"'{listIdString}' is not a valid option.");
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
                ConsoleService.Error("The task could not be deleted.");
            }

            await RefreshLocalTasks();
        }

        public async Task Update(string listIdString)
        {
            int listId;
            var task = new TaskResponse();
            try
            {
                listId = int.Parse(listIdString);
                task = tasks[listId - 1];
            }
            catch (Exception)
            {
                ConsoleService.Error($"'{listIdString}' is not a valid option.");
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
                        ConsoleService.Error($"'{typeChoice}' is not a valid choice.");
                        return;
                    }
                    break;
                case "n":
                    break;
                default:
                    ConsoleService.Error($"'{typeInput}' is not a valid choice.");
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
                        ConsoleService.Error($"'{statusChoice}' is not a valid choice.");
                        return;
                    }
                    break;
                case "n":
                    break;
                default:
                    ConsoleService.Error($"'{statusInput}' is not a valid choice.");
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
                                ConsoleService.Error($"Please select a due date that is past the date the task was created ({task.dateCreated.ToString(format)}).");
                                return;
                            }
                        }
                    }
                    catch (FormatException)
                    {
                        ConsoleService.Error("The due date provided is invalid.");
                        return;
                    }
                    break;
                case "n":
                    break;
                default:
                    ConsoleService.Error($"'{dueDateInput}' is not a valid choice.");
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
                    throw new Exception("Failed to update the task.");
                }
            }
            catch
            {
                ConsoleService.Error("Failed to update the task.");
                return;
            }

            await RefreshLocalTasks();
        }

        public async Task Comment(string listIdString)
        {
            int listId;
            TaskResponse? task;
            try
            {
                listId = int.Parse(listIdString);
                task = tasks[listId - 1];
            }
            catch (Exception)
            {
                ConsoleService.Error($"'{listIdString}' is not a valid option.");
                return;
            }

            Console.Write("Enter your comment: ");
            string commentString = Console.ReadLine()?.Trim() ?? "";
            if (string.IsNullOrEmpty(commentString))
            {
                ConsoleService.Error("The comment cannot be empty.");
                return;
            }
            if (commentString.Length > 100)
            {
                ConsoleService.Error("The comment cannot be more than 100 characters");
                return;
            }

            var currentDate = DateTime.Now;
            var comment = new CommentCreate
            {
                taskId = task.taskId,
                comment = commentString,
                dateCommented = new DateTime(
                    currentDate.Year,
                    currentDate.Month,
                    currentDate.Day,
                    currentDate.Hour,
                    currentDate.Minute,
                    0
                ),
            };

            try
            {
                var request = new HttpRequestMessage(HttpMethod.Post, "http://dev-do-list-backend.eu-west-1.elasticbeanstalk.com/api/v1/comment");
                request.Content = new StringContent(JsonSerializer.Serialize(comment), Encoding.UTF8, "application/json");
                var response = await client.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine("Comment added succesfully!");
                }
                else
                {
                    throw new Exception("Unable to upload comment.");
                }
            }
            catch(Exception)
            {
                ConsoleService.Error("Unable to upload comment.");
                return;
            }
            await RefreshLocalTasks();
        }

        private readonly HttpClient client = new();

        private readonly string format = "dd/MM/yyyy HH:mm";
            
        private readonly CultureInfo culture = new("en-GB");

        private List<TaskResponse> tasks = [];

        private List<StatusResponse> statuses = [];

        private List<TaskTypeResponse> types = [];
    }
}
