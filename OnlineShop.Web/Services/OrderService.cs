using System.Net.Http.Json;
using OnlineShop.Shared.DTOs;

namespace OnlineShop.Web.Services;

public class OrderService
{
    private readonly HttpClient _http;
    private readonly AuthService _authService;

    public OrderService(HttpClient http, AuthService authService)
    {
        _http = http;
        _authService = authService;
    }

    private async Task SetAuthHeader()
    {
        var token = await _authService.GetToken();
        _http.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
    }

    public async Task<bool> CreateOrder()
    {
        await SetAuthHeader();
        var response = await _http.PostAsync("api/orders", null);
        return response.IsSuccessStatusCode;
    }

    public async Task<List<OrderDto>> GetMyOrders()
    {
        await SetAuthHeader();
        var result = await _http.GetFromJsonAsync<List<OrderDto>>("api/orders/my");
        return result ?? new List<OrderDto>();
    }

    public async Task<List<OrderDto>> GetAllOrders()
    {
        await SetAuthHeader();
        var result = await _http.GetFromJsonAsync<List<OrderDto>>("api/orders");
        return result ?? new List<OrderDto>();
    }

    public async Task<bool> UpdateStatus(int id, UpdateOrderStatusDto dto)
    {
        await SetAuthHeader();
        var response = await _http.PatchAsJsonAsync($"api/orders/{id}/status", dto);
        return response.IsSuccessStatusCode;
    }
}