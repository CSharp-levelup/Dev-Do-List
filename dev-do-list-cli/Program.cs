using dev_do_list_cli.Services;

Console.ForegroundColor = ConsoleColor.Blue;
Console.WriteLine(" __  __       __  __        _____ ");
Console.WriteLine("|  \\|_ \\  /  |  \\/  \\  |  |(_  |  ");
Console.WriteLine("|__/|__ \\/   |__/\\__/  |__|__) |  ");
Console.ResetColor();

bool loggedIn = false;

help();
while (!loggedIn)
{
    Console.Write("\n$ ");
    string? choice = Console.ReadLine()?.Trim();
    if (string.IsNullOrEmpty(choice))
    {
        continue;
    }
    var choiceParameters = choice.Split(" ");

    try
    {
        switch (choiceParameters[0].Trim())
        {
            case "help":
                if (choiceParameters.Count() != 1)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Error: Too many arguments. 'help' does not take in any arguments");
                    Console.ResetColor();
                    break;
                }
                help();
                break;
            case "login":
                if (choiceParameters.Count() != 1)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Error: Too many arguments. 'login' does not take in any arguments");
                    Console.ResetColor();
                    break;
                }
                await LoginService.HandleLogin();
                loggedIn = true;
                break;
            case "exit":
                if (choiceParameters.Count() != 1)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Error: Too many arguments. 'exit' does not take in any arguments");
                    Console.ResetColor();
                    break;
                }
                exit();
                break;
            default:
                Console.WriteLine("Invalid input. Please enter a valid command. Type in 'help' to see all available commands");
                break;
        }
    }
    catch(Exception)
    { 
        Console.WriteLine("Something went wrong during the login process, please try again.");
    }
}

var taskService = new TaskService();
await taskService.RefreshLocalStatuses();
await taskService.RefreshLocalTypes();
await taskService.RefreshLocalTasks();

help();
while (true)
{
    Console.Write("\n$ ");
    string? choice = Console.ReadLine()?.Trim();
    if (string.IsNullOrEmpty(choice))
    {
        continue;
    }
    var choiceParameters = choice.Split(" ");

    switch (choiceParameters[0].Trim())
    {
        case "help":
            if (choiceParameters.Count() != 1)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Error: Too many arguments. 'help' does not take in any arguments");
                Console.ResetColor();
                break;
            }
            help();
            break;
        case "list":
            if (choiceParameters.Count() != 1)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Error: Too many arguments. 'list' does not take in any arguments");
                Console.ResetColor();
                break;
            }
            await taskService.List();
            break;
        case "details":
            if (choiceParameters.Count() != 2)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Error: Incorrect number of arguments. Please only provide the id of the task you wish to see in more detail");
                Console.ResetColor();
                break;
            }
            taskService.Details(choiceParameters[1].Trim());
            break;
        case "add":
            if (choiceParameters.Count() != 1)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Error: Too many arguments. 'create' does not take in any arguments");
                Console.ResetColor();
                break;
            }
            await taskService.Create();
            break;
        case "delete":
            if (choiceParameters.Count() != 2)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Error: Incorrect number of arguments. Please only provide the id of the task you wish to delete");
                Console.ResetColor();
                break;
            }
            await taskService.Delete(choiceParameters[1].Trim());
            break;
        case "update":
            // Logic for updating a task's status
            break;
        case "comment":
            // Logic for adding a comment to a task
            break;
        case "exit":
            exit();
            break;
        default:
            Console.WriteLine("Invalid input. Please enter a valid command. Type in 'help' to see all available commands");
            break;
    }
}

void help()
{
    if (!loggedIn)
    {
        Console.WriteLine("\nhelp - Show available commands");
        Console.WriteLine("login - Login with github");
        Console.WriteLine("exit - Exit the application");
    }
    else
    {
        Console.WriteLine("\nhelp - Show available commands");
        Console.WriteLine("list - List all tasks");
        Console.WriteLine("details - Get details of a task");
        Console.WriteLine("add - Add a new task");
        Console.WriteLine("delete - Delete a task");
        Console.WriteLine("update - Update a task's status");
        Console.WriteLine("comment - Add a comment to a task");
        Console.WriteLine("exit - Exit the application");
    }
}

static void exit()
{
    Console.WriteLine("Exiting...");
    Environment.Exit(0);
}