namespace OnlineShop.Web.Services;

public class OrderService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly AuthService _authService;

    public OrderService(IHttpClientFactory httpClientFactory, AuthService authService)
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

    public async Task<(bool success, string? error)> CreateOrder()
    {
        var client = await GetClient();
        var response = await client.PostAsync("api/orders", null);

        if (!response.IsSuccessStatusCode)
        {
            var errorMessage = await response.Content.ReadAsStringAsync();
            return (false, string.IsNullOrWhiteSpace(errorMessage) ? "Ошибка при создании заказа" : errorMessage);
        }

        return (true, null);
    }

    public async Task<List<OrderDto>> GetMyOrders()
    {
        var client = await GetClient();
        var result = await client.GetFromJsonAsync<List<OrderDto>>("api/orders/my");
        return result ?? new List<OrderDto>();
    }

    public async Task<List<OrderDto>> GetAllOrders()
    {
        var client = await GetClient();
        var result = await client.GetFromJsonAsync<List<OrderDto>>("api/orders");
        return result ?? new List<OrderDto>();
    }

    public async Task<bool> UpdateStatus(int id, UpdateOrderStatusDto dto)
    {
        var client = await GetClient();
        var response = await client.PatchAsJsonAsync($"api/orders/{id}/status", dto);
        return response.IsSuccessStatusCode;
    }
}