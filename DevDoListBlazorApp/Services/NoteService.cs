using System.Collections.Generic;
using System.Text.Json;
using DevDoListBlazorApp.Models;
using DevDoListBlazorApp.Utils;

namespace DevDoListBlazorApp.Services;

public class NoteService(string accessToken)
{
    private readonly string _serverUrl = FuncUtils.GetServerUrl();

    public async Task<List<Note>?> GetAllNotes()
    {
        var client = new HttpClient();
        var request = new HttpRequestMessage(HttpMethod.Get, _serverUrl + "api/v1//task");
        request.Headers.Add("Authorization", "Bearer " + accessToken);
        var response = await client.SendAsync(request);
        if (!response.IsSuccessStatusCode) return null;
        var task = JsonSerializer.Deserialize <List<Note>> (await response.Content.ReadAsStringAsync())!;
        return task;

    }
}