using System.Net.Http.Json;
using System.Text.Json;
using StudyConnect.Web.Models;

namespace StudyConnect.Web.Services;

public class ClientGroupService : IGroupService
{
    private readonly HttpClient _httpClient;

    public ClientGroupService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<StudyGroupDto>> GetMyGroupsAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync("api/groups");
            if (!response.IsSuccessStatusCode) return new List<StudyGroupDto>();

            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<StudyGroupDto>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true })
                ?? new List<StudyGroupDto>();
        }
        catch
        {
            return new List<StudyGroupDto>();
        }
    }

    public async Task<StudyGroupDto?> CreateGroupAsync(CreateGroupRequest request)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("api/groups", request);
            if (!response.IsSuccessStatusCode) return null;

            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<StudyGroupDto>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }
        catch
        {
            return null;
        }
    }

    public async Task<StudyGroupDto?> JoinGroupAsync(JoinGroupRequest request)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("api/groups/join", request);
            if (!response.IsSuccessStatusCode) return null;

            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<StudyGroupDto>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }
        catch
        {
            return null;
        }
    }

    public async Task<GroupDetailDto?> GetGroupDetailAsync(Guid groupId)
    {
        try
        {
            var response = await _httpClient.GetAsync($"api/groups/{groupId}");
            if (!response.IsSuccessStatusCode) return null;

            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<GroupDetailDto>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }
        catch
        {
            return null;
        }
    }

    public async Task<bool> LeaveGroupAsync(Guid groupId)
    {
        try
        {
            var response = await _httpClient.PostAsync($"api/groups/{groupId}/leave", null);
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> DeleteGroupAsync(Guid groupId)
    {
        try
        {
            var response = await _httpClient.DeleteAsync($"api/groups/{groupId}");
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }
}
