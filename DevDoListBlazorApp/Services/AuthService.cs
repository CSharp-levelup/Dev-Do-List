namespace DevDoListBlazorApp.Services;

public static class AuthService
{
    public static string? accessToken { get; set; }

    public static bool IsLoggedIn()
    {
        return accessToken is not null;
    }
}
