namespace OnlineShop.Web.Services;

public class UserService : BaseApiService
{
    public UserService(IHttpClientFactory httpClientFactory, AuthService authService)
        : base(httpClientFactory, authService) { }

    public async Task<UserProfileDto?> GetProfile()
    {
        try
        {
            var client = await GetClient(true);
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
            var client = await GetClient(true);
            var response = await client.PutAsJsonAsync("api/user/change-password", dto);
            return response.IsSuccessStatusCode;
        }
        catch { return false; }
    }

    public async Task<List<UserAdminDto>> GetAllUsers()
    {
        try
        {
            var client = await GetClient(true);
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
            var client = await GetClient(true);
            var response = await client.PutAsJsonAsync($"api/user/{userId}/role", new ChangeRoleDto { Role = role });
            return response.IsSuccessStatusCode;
        }
        catch { return false; }
    }
}