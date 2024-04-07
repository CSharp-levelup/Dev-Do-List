using dev_do_list_cli.Services;

Console.ForegroundColor = ConsoleColor.Blue;
Console.WriteLine(" __  __       __  __        _____ ");
Console.WriteLine("|  \\|_ \\  /  |  \\/  \\  |  |(_  |  ");
Console.WriteLine("|__/|__ \\/   |__/\\__/  |__|__) |  ");
Console.ResetColor();

bool loggedIn = false;
var taskService = new TaskService();

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
                help();
                break;
            case "login":
                await LoginService.HandleLogin();
                loggedIn = true;
                break;
            case "exit":
                exit();
                break;
            default:
                Console.WriteLine("Invalid input. Please enter the number of the option you would like to choose.");
                break;
        }
    }
    catch(Exception)
    { 
        Console.WriteLine("Something went wrong during the login process, please try again.");
    }
}

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
            help();
            break;
        case "list":
            // Logic for getting all tasks for a user
            break;
        case "details":
            // Logic for adding a task
            break;
        case "add":
            await taskService.Create();
            break;
        case "delete":
            // Logic for deleting a task
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
            Console.WriteLine("Invalid input. Please enter the number of the option you would like to choose.");
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