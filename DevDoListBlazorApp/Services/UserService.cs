using System.Text.Json;
using DevDoListBlazorApp.Models;
using DevDoListBlazorApp.Utils;

namespace DevDoListBlazorApp.Services;

public class UserService
{
    private readonly string _serverUrl = FuncUtils.GetServerUrl();

    public async Task<User?> GetUserByUsername()
    {
        var client = new HttpClient();
        var request = new HttpRequestMessage(HttpMethod.Get, _serverUrl + "api/v1/user/loggedIn");
        request.Headers.Add("Authorization", "Bearer " + AuthService.accessToken);
        var response = await client.SendAsync(request);
        if (!response.IsSuccessStatusCode) return null;
        var user = JsonSerializer.Deserialize<User>(await response.Content.ReadAsStringAsync())!;
        return user;

    }
}
