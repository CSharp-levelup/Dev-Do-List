using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;

namespace DevDoListBlazorApp.Services;

public class AuthService(ProtectedLocalStorage protectedLocalStorage)
{
    private readonly string _accessTokenKey = "JWT_TOKEN";

    public async Task<string?> GetAccessToken()
    {
        return (await protectedLocalStorage.GetAsync<string>(_accessTokenKey)).Value;
    }

    public async Task SetAccessToken(string token)
    {
        await protectedLocalStorage.SetAsync(_accessTokenKey, token);
    }

    public async Task<bool> IsLoggedIn()
    {
        return (await protectedLocalStorage.GetAsync<string>(_accessTokenKey)).Success;
    }
}
