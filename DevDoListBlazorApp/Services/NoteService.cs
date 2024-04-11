using System.Net.Http.Headers;
using System.Text.Json;
using DevDoListBlazorApp.Models;
using DevDoListBlazorApp.Utils;

namespace DevDoListBlazorApp.Services;

public class NoteService(HttpClient client)
{
    private readonly string _serverUrl = FuncUtils.GetServerUrl();

    public async Task<List<Note>?> GetAllNotes()
    {
        var request = new HttpRequestMessage(HttpMethod.Get, _serverUrl + "api/v1/task");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", AuthService.accessToken);
        var response = await client.SendAsync(request);
        if (!response.IsSuccessStatusCode) return null;
        var task = JsonSerializer.Deserialize<List<Note>>(await response.Content.ReadAsStringAsync())!;
        return task;
    }

    public async Task<Note?> GetNoteById(int id)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, _serverUrl + "api/v1/task/" + id);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", AuthService.accessToken);
        var response = await client.SendAsync(request);
        if (!response.IsSuccessStatusCode) return null;
        var task = JsonSerializer.Deserialize<Note>(await response.Content.ReadAsStringAsync())!;
        return task;
    }

    public async Task<Note?> UpdateNote(Note newTask, int id)
    {
        var request = new HttpRequestMessage(HttpMethod.Put, _serverUrl + "api/v1/task/" + id);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", AuthService.accessToken);
        var data = new
        {
            taskId = id,
            userId = newTask.userId,
            title = newTask.title,
            description = newTask.description,
            dueDate = newTask.dueDate,
            statusId = newTask.statusId,
            taskTypeId = newTask.taskTypeId
        };
        var content = new StringContent(
               JsonSerializer.Serialize(data),
               null,
               "application/json"
           );
        request.Content = content;
        var response = await client.SendAsync(request);
        if (!response.IsSuccessStatusCode) return null;
        var newNote = JsonSerializer.Deserialize<Note>(await response.Content.ReadAsStringAsync());
        return newNote;
    }

    public async Task<bool> DeleteNote(int id)
    {
        var request = new HttpRequestMessage(HttpMethod.Delete, _serverUrl + "api/v1/task/" + id);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", AuthService.accessToken);
        
        var response = await client.SendAsync(request);
        return response.IsSuccessStatusCode;
    }

    public async Task<Note?> CreateNote(Note newTask)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, _serverUrl + "api/v1/task");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", AuthService.accessToken);
        var data = new
        {
            title = newTask.title,
            description = newTask.description,
            dateCreated = DateTime.Now,
            dueDate = newTask.dueDate,
            statusId = newTask.statusId,
            taskTypeId = newTask.taskTypeId
        };
        var content = new StringContent(
               JsonSerializer.Serialize(data),
               null,
               "application/json"
           );
        request.Content = content;
        var response = await client.SendAsync(request);
        if (!response.IsSuccessStatusCode) return null;
        var newNote = JsonSerializer.Deserialize<Note>(await response.Content.ReadAsStringAsync());
        return newNote;
    }
}
