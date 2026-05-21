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
        if (!string.IsNullOrEmpty(token))
            client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        return client;
    }

    public async Task<UserProfileDto?> GetProfile()
    {
        try
        {
            var client = await GetClient();
            var response = await client.GetAsync("api/user/profile");
            if (!response.IsSuccessStatusCode) return null;
            return await response.Content.ReadFromJsonAsync<UserProfileDto>();
        }
        catch { return null; }
    }

    public async Task<bool> ChangePassword(ChangePasswordDto dto)
    {
        try
        {
            var client = await GetClient();
            var response = await client.PutAsJsonAsync("api/user/change-password", dto);
            return response.IsSuccessStatusCode;
        }
        catch { return false; }
    }

    public async Task<List<UserAdminDto>> GetAllUsers()
    {
        try
        {
            var client = await GetClient();
            var response = await client.GetAsync("api/user/all");
            if (!response.IsSuccessStatusCode) return new();
            return await response.Content.ReadFromJsonAsync<List<UserAdminDto>>() ?? new();
        }
        catch { return new(); }
    }

    public async Task<bool> ChangeRole(int userId, string role)
    {
        try
        {
            var client = await GetClient();
            var response = await client.PutAsJsonAsync($"api/user/{userId}/role", new ChangeRoleDto { Role = role });
            return response.IsSuccessStatusCode;
        }
        catch { return false; }
    }
}