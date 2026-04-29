using System.Net.Http.Json;
using OnlineShop.Shared.DTOs;


namespace OnlineShop.Web.Services;

public class ProductService
{
    private readonly HttpClient _http;
    private readonly AuthService _authService;

    public ProductService(HttpClient http, AuthService authService)
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

    public async Task<List<ProductDto>> GetAll(int? categoryId = null, string? search = null)
    {
        var url = "api/products";
        var query = new List<string>();

        if (categoryId.HasValue)
            query.Add($"categoryId={categoryId}");
        if (!string.IsNullOrEmpty(search))
            query.Add($"search={search}");
        if (query.Any())
            url += "?" + string.Join("&", query);

        var result = await _http.GetFromJsonAsync<List<ProductDto>>(url);
        return result ?? new List<ProductDto>();
    }

    public async Task<ProductDto?> GetById(int id)
    {
        return await _http.GetFromJsonAsync<ProductDto>($"api/products/{id}");
    }

    public async Task<bool> Create(CreateProductDto dto)
    {
        await SetAuthHeader();
        var response = await _http.PostAsJsonAsync("api/products", dto);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> Update(int id, CreateProductDto dto)
    {
        await SetAuthHeader();
        var response = await _http.PutAsJsonAsync($"api/products/{id}", dto);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> Delete(int id)
    {
        await SetAuthHeader();
        var response = await _http.DeleteAsync($"api/products/{id}");
        return response.IsSuccessStatusCode;
    }
}