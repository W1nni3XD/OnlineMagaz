using System.Net.Http.Json;
using OnlineShop.Shared.DTOs;

namespace OnlineShop.Web.Services;

public class CategoryService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly AuthService _authService;

    public CategoryService(IHttpClientFactory httpClientFactory, AuthService authService)
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

    public async Task<List<CategoryDto>> GetAll()
    {
        var client = await GetClient();
        var result = await client.GetFromJsonAsync<List<CategoryDto>>("api/categories");
        return result ?? new List<CategoryDto>();
    }

    /// <summary>Общие категории + свои (JWT).</summary>
    public async Task<List<CategoryDto>> GetAvailableForSeller()
    {
        var client = await GetClient(true);
        var result = await client.GetFromJsonAsync<List<CategoryDto>>("api/categories/available");
        return result ?? new List<CategoryDto>();
    }

    public async Task<bool> Create(CategoryDto dto)
    {
        var client = await GetClient(true);
        var response = await client.PostAsJsonAsync("api/categories", dto);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> Update(int id, CategoryDto dto)
    {
        var client = await GetClient(true);
        var response = await client.PutAsJsonAsync($"api/categories/{id}", dto);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> Delete(int id)
    {
        var client = await GetClient(true);
        var response = await client.DeleteAsync($"api/categories/{id}");
        return response.IsSuccessStatusCode;
    }
}