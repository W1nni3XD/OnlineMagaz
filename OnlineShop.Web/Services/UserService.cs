using System.Net.Http.Json;
using OnlineShop.Domain.DTOs;

namespace OnlineShop.Web.Services;

public class UserService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly AuthService _authService;

    public UserService(IHttpClientFactory httpClientFactory, AuthService authService)
    {
        _httpClientFactory = httpClientFactory;
        _authService = authService;
    }

    private async Task<HttpClient> GetClient()
    {
        var client = _httpClientFactory.CreateClient("API");
        var token = await _authService.GetToken();
        client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        return client;
    }

    public async Task<UserProfileDto?> GetProfile()
    {
        var client = await GetClient();
        return await client.GetFromJsonAsync<UserProfileDto>("api/user/profile");
    }

    public async Task<(bool success, string? error)> ChangePassword(ChangePasswordDto dto)
    {
        var client = await GetClient();
        var response = await client.PutAsJsonAsync("api/user/change-password", dto);

        if (!response.IsSuccessStatusCode)
        {
            var errorMessage = await response.Content.ReadAsStringAsync();
            return (false, string.IsNullOrWhiteSpace(errorMessage) ? "Ошибка при смене пароля" : errorMessage);
        }

        return (true, null);
    }
}
