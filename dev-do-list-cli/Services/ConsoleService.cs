namespace dev_do_list_cli.Services
{
    public static class ConsoleService
    {
        public static void Error(string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Error: {message}");
            Console.ResetColor();
        }
    }
}
