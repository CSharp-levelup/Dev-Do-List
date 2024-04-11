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
                if (choiceParameters.Length != 1)
                {
                    ConsoleService.Error("Too many arguments. 'help' does not take in any arguments.");
                    break;
                }
                help();
                break;
            case "login":
                if (choiceParameters.Length != 1)
                {
                    ConsoleService.Error("Too many arguments. 'login' does not take in any arguments.");
                    break;
                }
                await LoginService.HandleLogin();
                loggedIn = true;
                break;
            case "exit":
                if (choiceParameters.Length != 1)
                {
                    ConsoleService.Error("Too many arguments. 'exit' does not take in any arguments.");
                    break;
                }
                exit();
                break;
            default:
                ConsoleService.Error("Invalid input. Please enter a valid command. Type in 'help' to see all available commands.");
                break;
        }
    }
    catch(Exception)
    { 
        ConsoleService.Error("Something went wrong during the login process, please try again.");
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
            if (choiceParameters.Length != 1)
            {
                ConsoleService.Error("Too many arguments. 'help' does not take in any arguments;");
                break;
            }
            help();
            break;
        case "list":
            if (choiceParameters.Length != 1)
            {
                ConsoleService.Error("Too many arguments. 'list' does not take in any arguments;");
                break;
            }
            await taskService.List();
            break;
        case "details":
            if (choiceParameters.Length != 2)
            {
                ConsoleService.Error("Incorrect number of arguments. Please only provide the id of the task you wish to see in more detail.");
                break;
            }
            taskService.Details(choiceParameters[1].Trim());
            break;
        case "add":
            if (choiceParameters.Length != 1)
            {
                ConsoleService.Error("Too many arguments. 'create' does not take in any arguments");
                break;
            }
            await taskService.Create();
            break;
        case "delete":
            if (choiceParameters.Length != 2)
            {
                ConsoleService.Error("Incorrect number of arguments. Please only provide the id of the task you wish to delete.");
                break;
            }
            await taskService.Delete(choiceParameters[1].Trim());
            break;
        case "update":
            if (choiceParameters.Length != 2)
            {
                ConsoleService.Error("Incorrect number of arguments. Please only provide the id of the task you wish to delete.");
                break;
            }
            await taskService.Update(choiceParameters[1].Trim());
            break;
        case "comment":
            if (choiceParameters.Length != 2)
            {
                ConsoleService.Error("Incorrect number of arguments. Please only provide the id of the task you wish to comment on.");
                break;
            }
            await taskService.Comment(choiceParameters[1].Trim());
            break;
        case "profile":
            if (choiceParameters.Length != 1)
            {
                ConsoleService.Error("Too many arguments. 'profile' does not take in any arguments.");
            }
            Console.WriteLine($"You are currently signed in as {UserService.Username}!");
            break;
        case "exit":
            exit();
            break;
        default:
            ConsoleService.Error("Invalid input. Please enter a valid command. Type in 'help' to see all available commands.");
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
        Console.WriteLine("details <id> - Get details of a task");
        Console.WriteLine("add - Add a new task");
        Console.WriteLine("delete <id> - Delete a task");
        Console.WriteLine("update <id> - Update a task's status");
        Console.WriteLine("comment <id> - Add a comment to a task");
        Console.WriteLine("profile - See which profile you are signed in to");
        Console.WriteLine("exit - Exit the application");
    }
}

static void exit()
{
    Console.WriteLine("Exiting...");
    Environment.Exit(0);
}
