using Smooth.Flaunt.Models;
using System.Net.Http.Json;

namespace Smooth.Flaunt;

public class AuthenticationService
{
    private readonly HttpClient _httpClient;

    public AuthenticationService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task LoginAsync()
    {
        await _httpClient.GetAsync("bff/login"); // Redirects to login page
    }

    public async Task LogoutAsync()
    {
        await _httpClient.GetAsync("bff/logout"); // Logs out from all sessions
    }

    public async Task<UserInfo> GetUserAsync()
    {
        var response = await _httpClient.GetFromJsonAsync<UserInfo>("bff/user");
        return response ?? new UserInfo();
    }
}
