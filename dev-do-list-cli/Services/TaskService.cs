﻿using System.Globalization;
using System.Text;
using System.Text.Json;
using dev_do_list_cli.Models;

namespace dev_do_list_cli.Services
{
    public class TaskService
    {
        public async Task RefreshLocalTasks()
        {
            using HttpClient client = new();
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", LoginService.JwtToken);

            // Fetch all tasks
            var taskRequest = new HttpRequestMessage(HttpMethod.Get, $"http://dev-do-list-backend.eu-west-1.elasticbeanstalk.com/api/v1/task");
            var taskResponse = client.Send(taskRequest);

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
            using HttpClient client = new();
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", LoginService.JwtToken);

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
            using HttpClient client = new();
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", LoginService.JwtToken);

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

            using HttpClient client = new();
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", LoginService.JwtToken);

            task.userId = UserService.UserId;

            CultureInfo culture = new CultureInfo("en-GB");
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
            string format = "dd/MM/yyyy HH:mm";
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
                    Console.WriteLine("Succesfully created task!");
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
            using HttpClient client = new();
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", LoginService.JwtToken);
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

        private List<TaskResponse> tasks = new List<TaskResponse>();
        private List<StatusResponse> statuses = new List<StatusResponse>();
        private List<TaskTypeResponse> types = new List<TaskTypeResponse>();
    }
}
