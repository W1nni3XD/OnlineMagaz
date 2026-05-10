using System.Net.Http.Json;
using OnlineShop.Shared.DTOs;

namespace OnlineShop.Web.Services;

public class ProductService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly AuthService _authService;

    public ProductService(IHttpClientFactory httpClientFactory, AuthService authService)
    {
        _httpClientFactory = httpClientFactory;
        _authService = authService;
    }

    private async Task<HttpClient> GetClient(bool withAuth = false)
    {
        var client = _httpClientFactory.CreateClient("API");
        if (withAuth)
        {
            var token = await _authService.GetToken();
            client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        }
        return client;
    }

    public async Task<List<ProductDto>> GetAll(int? categoryId = null, string? search = null)
    {
        var client = await GetClient();
        var url = "api/products";
        var query = new List<string>();
        if (categoryId.HasValue) query.Add($"categoryId={categoryId}");
        if (!string.IsNullOrEmpty(search)) query.Add($"search={search}");
        if (query.Any()) url += "?" + string.Join("&", query);
        var result = await client.GetFromJsonAsync<List<ProductDto>>(url);
        return result ?? new List<ProductDto>();
    }

    public async Task<ProductDto?> GetById(int id)
    {
        var client = await GetClient();
        return await client.GetFromJsonAsync<ProductDto>($"api/products/{id}");
    }

    /// <summary>Товары текущего продавца (требуется JWT).</summary>
    public async Task<List<ProductDto>> GetMine()
    {
        var client = await GetClient(true);
        var result = await client.GetFromJsonAsync<List<ProductDto>>("api/products/mine");
        return result ?? new List<ProductDto>();
    }

    public async Task<bool> Create(CreateProductDto dto)
    {
        var client = await GetClient(true);
        var response = await client.PostAsJsonAsync("api/products", dto);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> Update(int id, CreateProductDto dto)
    {
        var client = await GetClient(true);
        var response = await client.PutAsJsonAsync($"api/products/{id}", dto);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> Delete(int id)
    {
        var client = await GetClient(true);
        var response = await client.DeleteAsync($"api/products/{id}");
        return response.IsSuccessStatusCode;
    }
}
