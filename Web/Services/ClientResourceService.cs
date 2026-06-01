using System.Net.Http.Json;
using System.Text.Json;
using StudyConnect.Web.Models;

namespace StudyConnect.Web.Services;

public class ClientResourceService : IResourceService
{
    private readonly HttpClient _httpClient;

    public ClientResourceService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<SharedResourceDto>> GetGroupResourcesAsync(Guid groupId)
    {
        try
        {
            var response = await _httpClient.GetAsync($"api/groups/{groupId}/resources");
            if (!response.IsSuccessStatusCode) return new List<SharedResourceDto>();

            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<SharedResourceDto>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true })
                ?? new List<SharedResourceDto>();
        }
        catch
        {
            return new List<SharedResourceDto>();
        }
    }

    public async Task<SharedResourceDto?> CreateResourceAsync(Guid groupId, CreateResourceRequest request)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync($"api/groups/{groupId}/resources", request);
            if (!response.IsSuccessStatusCode) return null;

            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<SharedResourceDto>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }
        catch
        {
            return null;
        }
    }

    public async Task<SharedResourceDto?> UpdateResourceAsync(Guid resourceId, Guid groupId, UpdateResourceRequest request)
    {
        try
        {
            var response = await _httpClient.PutAsJsonAsync($"api/groups/{groupId}/resources/{resourceId}", request);
            if (!response.IsSuccessStatusCode) return null;

            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<SharedResourceDto>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }
        catch
        {
            return null;
        }
    }

    public async Task<bool> DeleteResourceAsync(Guid resourceId, Guid groupId)
    {
        try
        {
            var response = await _httpClient.DeleteAsync($"api/groups/{groupId}/resources/{resourceId}");
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }
}
