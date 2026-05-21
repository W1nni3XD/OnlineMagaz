namespace OnlineShop.Web.Services;

public class WishlistService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly AuthService _authService;

    public WishlistService(IHttpClientFactory httpClientFactory, AuthService authService)
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

    public async Task<List<WishlistItemDto>> GetWishlist()
    {
        try
        {
            var client = await GetClient();
            var response = await client.GetAsync("api/wishlist");
            if (!response.IsSuccessStatusCode)
                return new List<WishlistItemDto>();
            return await response.Content.ReadFromJsonAsync<List<WishlistItemDto>>()
                   ?? new List<WishlistItemDto>();
        }
        catch
        {
            return new List<WishlistItemDto>();
        }
    }

    public async Task<(bool success, string? error)> AddToWishlist(int productId)
    {
        try
        {
            var client = await GetClient();
            var response = await client.PostAsJsonAsync("api/wishlist", new AddToWishlistDto { ProductId = productId });
            if (!response.IsSuccessStatusCode)
            {
                var errorMessage = await response.Content.ReadAsStringAsync();
                return (false, string.IsNullOrWhiteSpace(errorMessage) ? "Ошибка при добавлении в избранное" : errorMessage);
            }
            return (true, null);
        }
        catch
        {
            return (false, "Ошибка соединения");
        }
    }

    public async Task<bool> RemoveFromWishlist(int id)
    {
        try
        {
            var client = await GetClient();
            var response = await client.DeleteAsync($"api/wishlist/{id}");
            return response.IsSuccessStatusCode;
        }
        catch { return false; }
    }

    public async Task<bool> RemoveByProductId(int productId)
    {
        try
        {
            var client = await GetClient();
            var response = await client.DeleteAsync($"api/wishlist/product/{productId}");
            return response.IsSuccessStatusCode;
        }
        catch { return false; }
    }
}