﻿using System.Collections.Generic;
using System.Text.Json;
using DevDoListBlazorApp.Models;
using DevDoListBlazorApp.Utils;
using DevDoListBlazorApp.Data;

namespace DevDoListBlazorApp.Services;

public class NoteService(string accessToken)
{
    private readonly string _serverUrl = FuncUtils.GetServerUrl();
    private readonly MockNotesData mockNotesData = new();

    public async Task<List<Note>?> GetAllNotes()
    {
        var client = new HttpClient();
        var request = new HttpRequestMessage(HttpMethod.Get, _serverUrl + "api/v1/task");
        request.Headers.Add("Authorization", "Bearer " + accessToken);
        var response = await client.SendAsync(request);
        if (!response.IsSuccessStatusCode) return null;
        var task = JsonSerializer.Deserialize<List<Note>>(await response.Content.ReadAsStringAsync())!;
        return task;
    }

    public async Task<bool> DeleteNote(int id)
    {
        var client = new HttpClient();
        var request = new HttpRequestMessage(HttpMethod.Delete, _serverUrl + "api/v1/task/" + id);
        request.Headers.Add("Authorization", "Bearer " + accessToken);
        
        var response = await client.SendAsync(request);
        return response.IsSuccessStatusCode;
    }

    public async Task<Note?> CreateNote(Note newTask)
    {
        var client = new HttpClient();
        var request = new HttpRequestMessage(HttpMethod.Post, _serverUrl + "api/v1/task");
        request.Headers.Add("Authorization", "Bearer " + accessToken);
        var data = new
        {
            title = newTask.title,
            description = newTask.description,
            dateCreated = DateTime.Now,
            dueDate = newTask.dueDate,
            userId = 1,
            statusId = newTask.statusId,
            taskTypeId = newTask.taskTypeId
        };
        var jsonData = JsonSerializer.Serialize(data);
        var content = new StringContent(jsonData);
        request.Content = content;
        var response = await client.SendAsync(request);
        if (!response.IsSuccessStatusCode) return null;
        var newNote = JsonSerializer.Deserialize<Note>(await response.Content.ReadAsStringAsync());
        return newNote;
    }

    public List<Note> GetAllNotesMock()
    {
        var task = mockNotesData.GenerateMockNotes();
        return task;

    }
}