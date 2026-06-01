using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;

namespace StudyConnect.Web.Services;

public class ClientAuthService : IAuthService
{
    private readonly HttpClient _httpClient;
    private readonly IJSRuntime _jsRuntime;
    private readonly CustomAuthStateProvider _authStateProvider;

    public ClientAuthService(HttpClient httpClient, IJSRuntime jsRuntime, AuthenticationStateProvider authStateProvider)
    {
        _httpClient = httpClient;
        _jsRuntime = jsRuntime;
        _authStateProvider = (CustomAuthStateProvider)authStateProvider;
    }

    public async Task<bool> RegisterAsync(Models.RegisterRequest request)
    {
        try
        {
            var payload = new { request.Name, request.Email, request.Password };
            var response = await _httpClient.PostAsJsonAsync("api/auth/register", payload);
            if (!response.IsSuccessStatusCode) return false;

            var content = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<Models.AuthResponse>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            if (result == null || string.IsNullOrEmpty(result.Token)) return false;

            await _jsRuntime.InvokeVoidAsync("localStorage.setItem", "auth_token", result.Token);
            _authStateProvider.NotifyUserAuthenticated(result.Token);
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> LoginAsync(Models.LoginRequest request)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("api/auth/login", request);
            if (!response.IsSuccessStatusCode) return false;

            var content = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<Models.AuthResponse>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            if (result == null || string.IsNullOrEmpty(result.Token)) return false;

            await _jsRuntime.InvokeVoidAsync("localStorage.setItem", "auth_token", result.Token);
            _authStateProvider.NotifyUserAuthenticated(result.Token);
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task LogoutAsync()
    {
        await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", "auth_token");
        _httpClient.DefaultRequestHeaders.Authorization = null;
        _authStateProvider.NotifyUserLoggedOut();
    }

    public async Task<Models.UserDto?> GetCurrentUserAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync("api/auth/me");
            if (!response.IsSuccessStatusCode) return null;

            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<Models.UserDto>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }
        catch
        {
            return null;
        }
    }
}
