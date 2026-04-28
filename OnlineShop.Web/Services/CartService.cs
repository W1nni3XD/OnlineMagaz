using System.Net.Http.Json;
using OnlineShop.Shared.DTOs;

namespace OnlineShop.Web.Services;

public class CartService
{
    private readonly HttpClient _http;
    private readonly AuthService _authService;

    public CartService(HttpClient http, AuthService authService)
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

    public async Task<List<CartItemDto>> GetCart()
    {
        await SetAuthHeader();
        var result = await _http.GetFromJsonAsync<List<CartItemDto>>("api/cart");
        return result ?? new List<CartItemDto>();
    }

    public async Task<bool> AddToCart(AddToCartDto dto)
    {
        await SetAuthHeader();
        var response = await _http.PostAsJsonAsync("api/cart", dto);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> UpdateQuantity(int id, UpdateCartItemDto dto)
    {
        await SetAuthHeader();
        var response = await _http.PutAsJsonAsync($"api/cart/{id}", dto);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> RemoveItem(int id)
    {
        await SetAuthHeader();
        var response = await _http.DeleteAsync($"api/cart/{id}");
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> ClearCart()
    {
        await SetAuthHeader();
        var response = await _http.DeleteAsync("api/cart");
        return response.IsSuccessStatusCode;
    }
}