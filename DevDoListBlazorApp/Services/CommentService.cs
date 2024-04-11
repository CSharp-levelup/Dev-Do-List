using System.Net.Http.Headers;
using System.Text.Json;
using DevDoListBlazorApp.Models;
using DevDoListBlazorApp.Utils;

namespace DevDoListBlazorApp.Services;

public class CommentService(HttpClient client, AuthService authService)
{
    private readonly string _serverUrl = FuncUtils.GetServerUrl();

    public async Task<List<Comment>?> GetUserComments(int taskId)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, _serverUrl + "api/v1/comment/task/" + taskId);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", authService.accessToken);
        var response = await client.SendAsync(request);
        if (!response.IsSuccessStatusCode) return null;
        var task = JsonSerializer.Deserialize<List<Comment>>(await response.Content.ReadAsStringAsync())!;
        return task;
    }

    public async Task<Comment?> GetCommentById(int commentId)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, _serverUrl + "api/v1/comment/" + commentId);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", authService.accessToken);
        var response = await client.SendAsync(request);
        if (!response.IsSuccessStatusCode) return null;
        var task = JsonSerializer.Deserialize<Comment>(await response.Content.ReadAsStringAsync())!;
        return task;
    }

    public async Task<Comment?> CreateCommentById(Comment newComment)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, _serverUrl + "api/v1/comment");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", authService.accessToken);
        var data = new
        {
            taskId = newComment.taskId,
            comment = newComment.comment,
            dateCommented = newComment.dateCommented
        };
        var content = new StringContent(
               JsonSerializer.Serialize(data),
               null,
               "application/json"
           );
        request.Content = content;
        var response = await client.SendAsync(request);
        if (!response.IsSuccessStatusCode) return null;
        var task = JsonSerializer.Deserialize<Comment>(await response.Content.ReadAsStringAsync());
        return task;
    }
}
