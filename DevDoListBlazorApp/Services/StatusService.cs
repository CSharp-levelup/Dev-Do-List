using System.Net.Http.Headers;
using System.Text.Json;
using DevDoListBlazorApp.Models;
using DevDoListBlazorApp.Utils;

namespace DevDoListBlazorApp.Services;

public class StatusService(HttpClient client, AuthService authService)
{
    private readonly string _serverUrl = FuncUtils.GetServerUrl();

    public async Task<List<Status>> GetAllStatuses()
    {
        var request = new HttpRequestMessage(HttpMethod.Get, _serverUrl + "api/v1/status");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", await authService.GetAccessToken());
        var response = await client.SendAsync(request);
        if (!response.IsSuccessStatusCode) return null;
        return JsonSerializer.Deserialize<List<Status>>(await response.Content.ReadAsStringAsync())!;
    }
}
