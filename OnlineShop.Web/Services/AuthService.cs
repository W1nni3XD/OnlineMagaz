using System.Net.Http.Json;
using Microsoft.JSInterop;
using OnlineShop.Domain.DTOs;

namespace OnlineShop.Web.Services;

public class AuthService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IJSRuntime _js;

    public AuthService(IHttpClientFactory httpClientFactory, IJSRuntime js)
    {
        _httpClientFactory = httpClientFactory;
        _js = js;
    }

    public async Task<(AuthResponseDto? response, string? error)> Register(RegisterDto dto)
    {
        var client = _httpClientFactory.CreateClient("API");
        var response = await client.PostAsJsonAsync("api/auth/register", dto);

        if (!response.IsSuccessStatusCode)
        {
            var errorMessage = await response.Content.ReadAsStringAsync();
            return (null, string.IsNullOrWhiteSpace(errorMessage) ? "Ошибка регистрации" : errorMessage);
        }

        var result = await response.Content.ReadFromJsonAsync<AuthResponseDto>();
        return (result, null);
    }

    public async Task<(AuthResponseDto? response, string? error)> Login(LoginDto dto)
    {
        var client = _httpClientFactory.CreateClient("API");
        var response = await client.PostAsJsonAsync("api/auth/login", dto);

        if (!response.IsSuccessStatusCode)
        {
            var errorMessage = await response.Content.ReadAsStringAsync();
            return (null, string.IsNullOrWhiteSpace(errorMessage) ? "Неверный email или пароль" : errorMessage);
        }

        var result = await response.Content.ReadFromJsonAsync<AuthResponseDto>();
        return (result, null);
    }

    public async Task SaveToken(string token)
    {
        await _js.InvokeVoidAsync("localStorage.setItem", "token", token);
    }

    public async Task<string?> GetToken()
    {
        return await _js.InvokeAsync<string?>("localStorage.getItem", "token");
    }

    public async Task RemoveToken()
    {
        await _js.InvokeVoidAsync("localStorage.removeItem", "token");
    }
}