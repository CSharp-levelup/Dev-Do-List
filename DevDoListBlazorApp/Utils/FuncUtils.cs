namespace DevDoListBlazorApp.Utils;

public class FuncUtils
{
    public static string GetServerUrl()
    {
        var serverUrl = Environment.GetEnvironmentVariable("DEVDOLIST_SERVER_URL");
        if (serverUrl is null)
        {
            throw new Exception("Server URL is null");
        }

        return serverUrl!;
    }
}