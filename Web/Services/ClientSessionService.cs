using System.Net.Http.Json;
using System.Text.Json;
using StudyConnect.Web.Models;

namespace StudyConnect.Web.Services;

public class ClientSessionService : ISessionService
{
    private readonly HttpClient _httpClient;

    public ClientSessionService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<SessionDto>> GetGroupSessionsAsync(Guid groupId)
    {
        try
        {
            var response = await _httpClient.GetAsync($"api/groups/{groupId}/sessions");
            if (!response.IsSuccessStatusCode) return new List<SessionDto>();

            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<SessionDto>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true })
                ?? new List<SessionDto>();
        }
        catch
        {
            return new List<SessionDto>();
        }
    }

    public async Task<SessionDto?> GetSessionByIdAsync(Guid sessionId)
    {
        try
        {
            var response = await _httpClient.GetAsync($"api/sessions/{sessionId}");
            if (!response.IsSuccessStatusCode) return null;

            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<SessionDto>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }
        catch
        {
            return null;
        }
    }

    public async Task<SessionDto?> CreateSessionAsync(Guid groupId, CreateSessionRequest request)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync($"api/groups/{groupId}/sessions", request);
            if (!response.IsSuccessStatusCode) return null;

            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<SessionDto>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }
        catch
        {
            return null;
        }
    }

    public async Task<SessionDto?> UpdateSessionAsync(Guid sessionId, Guid groupId, UpdateSessionRequest request)
    {
        try
        {
            var response = await _httpClient.PutAsJsonAsync($"api/groups/{groupId}/sessions/{sessionId}", request);
            if (!response.IsSuccessStatusCode) return null;

            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<SessionDto>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }
        catch
        {
            return null;
        }
    }

    public async Task<bool> DeleteSessionAsync(Guid sessionId, Guid groupId)
    {
        try
        {
            var response = await _httpClient.DeleteAsync($"api/groups/{groupId}/sessions/{sessionId}");
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }
}
