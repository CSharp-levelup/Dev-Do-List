namespace DevDoListBlazorApp.Services;

public class AuthService
{
    public string? accessToken { get; set; }

    public bool IsLoggedIn()
    {
        return accessToken is not null;
    }
}
