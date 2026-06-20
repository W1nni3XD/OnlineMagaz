namespace OnlineShop.Web.Services;

public class OrderService : BaseApiService
{
    public OrderService(IHttpClientFactory httpClientFactory, AuthService authService)
        : base(httpClientFactory, authService) { }

    public async Task<(bool success, string? error)> CreateOrder()
    {
        var client = await GetClient(true);
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
        var client = await GetClient(true);
        var result = await client.GetFromJsonAsync<List<OrderDto>>("api/orders/my");
        return result ?? new List<OrderDto>();
    }

    public async Task<List<OrderDto>> GetAllOrders()
    {
        var client = await GetClient(true);
        var result = await client.GetFromJsonAsync<List<OrderDto>>("api/orders");
        return result ?? new List<OrderDto>();
    }

    public async Task<bool> UpdateStatus(int id, UpdateOrderStatusDto dto)
    {
        var client = await GetClient(true);
        var response = await client.PatchAsJsonAsync($"api/orders/{id}/status", dto);
        return response.IsSuccessStatusCode;
    }
}