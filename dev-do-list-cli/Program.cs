using dev_do_list_cli.Services;

Console.WriteLine("Welcome to the Dev To Do List!\n");
Thread.Sleep(2000);

bool loggedIn = false;
var loginService = new LoginService();
while (!loggedIn)
{
    Console.WriteLine("\nDev-To-Do-List Options");
    Console.WriteLine("1. Login");
    Console.WriteLine("2. Exit");
    Console.Write("Enter your choice: ");

    string? choice = Console.ReadLine()?.Trim();

    Console.WriteLine("-----------------------------");

    try
    {
        switch (choice)
        {
            case "1":
                await loginService.HandleLogin();
                loggedIn = true;
                break;
            case "2":
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

while (true)
{
    Console.WriteLine("\nDev-To-Do-List Options");
    Console.WriteLine("1. View tasks");
    Console.WriteLine("2. Add a new task");
    Console.WriteLine("3. Update a task");
    Console.WriteLine("4. Delete a task");
    Console.WriteLine("5. Exit");
    Console.Write("Enter your choice: ");

    string? choice = Console.ReadLine()?.Trim();

    Console.WriteLine("-----------------------------");

    switch (choice)
    {
        case "1":
            // Logic for getting all tasks for a user
            break;
        case "2":
            // Logic for adding a task
            break;
        case "3":
            // Logic for updating a task
            break;
        case "4":
            // Logic for deleting a task
            break;
        case "5":
            exit();
            break;
        default:
            Console.WriteLine("Invalid input. Please enter the number of the option you would like to choose.");
            break;
    }
}

static void exit()
{
    Console.WriteLine("Exiting...");
    Environment.Exit(0);
}