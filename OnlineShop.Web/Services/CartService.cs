namespace OnlineShop.Web.Services;

public class CartService : BaseApiService
{
    public CartService(IHttpClientFactory httpClientFactory, AuthService authService)
        : base(httpClientFactory, authService) { }

    public async Task<List<CartItemDto>> GetCart()
    {
        var client = await GetClient(true);
        var result = await client.GetFromJsonAsync<List<CartItemDto>>("api/cart");
        return result ?? new List<CartItemDto>();
    }

    public async Task<bool> AddToCart(AddToCartDto dto)
    {
        var client = await GetClient(true);
        var response = await client.PostAsJsonAsync("api/cart", dto);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> UpdateQuantity(int id, UpdateCartItemDto dto)
    {
        var client = await GetClient(true);
        var response = await client.PutAsJsonAsync($"api/cart/{id}", dto);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> RemoveItem(int id)
    {
        var client = await GetClient(true);
        var response = await client.DeleteAsync($"api/cart/{id}");
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> ClearCart()
    {
        var client = await GetClient(true);
        var response = await client.DeleteAsync("api/cart");
        return response.IsSuccessStatusCode;
    }
}