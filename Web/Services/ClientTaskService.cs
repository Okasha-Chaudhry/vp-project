using System.Net.Http.Json;
using System.Text.Json;
using StudyConnect.Web.Models;

namespace StudyConnect.Web.Services;

public class ClientTaskService : ITaskService
{
    private readonly HttpClient _httpClient;

    public ClientTaskService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<GroupTaskDto>> GetGroupTasksAsync(Guid groupId)
    {
        try
        {
            var response = await _httpClient.GetAsync($"api/groups/{groupId}/tasks");
            if (!response.IsSuccessStatusCode) return new List<GroupTaskDto>();

            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<GroupTaskDto>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true })
                ?? new List<GroupTaskDto>();
        }
        catch
        {
            return new List<GroupTaskDto>();
        }
    }

    public async Task<List<GroupTaskDto>> GetMyTasksAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync("api/groups/00000000-0000-0000-0000-000000000000/tasks/my");
            if (!response.IsSuccessStatusCode) return new List<GroupTaskDto>();

            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<GroupTaskDto>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true })
                ?? new List<GroupTaskDto>();
        }
        catch
        {
            return new List<GroupTaskDto>();
        }
    }

    public async Task<GroupTaskDto?> CreateTaskAsync(Guid groupId, CreateTaskRequest request)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync($"api/groups/{groupId}/tasks", request);
            if (!response.IsSuccessStatusCode) return null;

            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<GroupTaskDto>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }
        catch
        {
            return null;
        }
    }

    public async Task<GroupTaskDto?> UpdateTaskAsync(Guid taskId, Guid groupId, UpdateTaskRequest request)
    {
        try
        {
            var response = await _httpClient.PutAsJsonAsync($"api/groups/{groupId}/tasks/{taskId}", request);
            if (!response.IsSuccessStatusCode) return null;

            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<GroupTaskDto>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }
        catch
        {
            return null;
        }
    }

    public async Task<bool> DeleteTaskAsync(Guid taskId, Guid groupId)
    {
        try
        {
            var response = await _httpClient.DeleteAsync($"api/groups/{groupId}/tasks/{taskId}");
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }
}
