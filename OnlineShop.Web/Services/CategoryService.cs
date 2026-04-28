using System.Net.Http.Json;
using OnlineShop.Shared.DTOs;

namespace OnlineShop.Web.Services;

public class CategoryService
{
    private readonly HttpClient _http;
    private readonly AuthService _authService;

    public CategoryService(HttpClient http, AuthService authService)
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

    public async Task<List<CategoryDto>> GetAll()
    {
        var result = await _http.GetFromJsonAsync<List<CategoryDto>>("api/categories");
        return result ?? new List<CategoryDto>();
    }

    public async Task<bool> Create(CategoryDto dto)
    {
        await SetAuthHeader();
        var response = await _http.PostAsJsonAsync("api/categories", dto);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> Update(int id, CategoryDto dto)
    {
        await SetAuthHeader();
        var response = await _http.PutAsJsonAsync($"api/categories/{id}", dto);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> Delete(int id)
    {
        await SetAuthHeader();
        var response = await _http.DeleteAsync($"api/categories/{id}");
        return response.IsSuccessStatusCode;
    }
}