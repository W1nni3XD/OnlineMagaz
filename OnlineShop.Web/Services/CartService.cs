using System.Net.Http.Json;
using OnlineShop.Shared.DTOs;

namespace OnlineShop.Web.Services;

public class CartService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly AuthService _authService;

    public CartService(IHttpClientFactory httpClientFactory, AuthService authService)
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

    public async Task<List<CartItemDto>> GetCart()
    {
        var client = await GetClient();
        var result = await client.GetFromJsonAsync<List<CartItemDto>>("api/cart");
        return result ?? new List<CartItemDto>();
    }

    public async Task<bool> AddToCart(AddToCartDto dto)
    {
        var client = await GetClient();
        var response = await client.PostAsJsonAsync("api/cart", dto);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> UpdateQuantity(int id, UpdateCartItemDto dto)
    {
        var client = await GetClient();
        var response = await client.PutAsJsonAsync($"api/cart/{id}", dto);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> RemoveItem(int id)
    {
        var client = await GetClient();
        var response = await client.DeleteAsync($"api/cart/{id}");
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> ClearCart()
    {
        var client = await GetClient();
        var response = await client.DeleteAsync("api/cart");
        return response.IsSuccessStatusCode;
    }
}