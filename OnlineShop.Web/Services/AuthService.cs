using System.Net.Http.Json;
using Microsoft.JSInterop;
using OnlineShop.Shared.DTOs;

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

    public async Task<AuthResponseDto?> Register(RegisterDto dto)
    {
        var client = _httpClientFactory.CreateClient("API");
        var response = await client.PostAsJsonAsync("api/auth/register", dto);
        if (!response.IsSuccessStatusCode) return null;
        return await response.Content.ReadFromJsonAsync<AuthResponseDto>();
    }

    public async Task<AuthResponseDto?> Login(LoginDto dto)
    {
        var client = _httpClientFactory.CreateClient("API");
        var response = await client.PostAsJsonAsync("api/auth/login", dto);
        if (!response.IsSuccessStatusCode) return null;
        return await response.Content.ReadFromJsonAsync<AuthResponseDto>();
    }

    public async Task SaveToken(string token)
    {
        await _js.InvokeVoidAsync("sessionStorage.setItem", "token", token);
    }

    public async Task<string?> GetToken()
    {
        return await _js.InvokeAsync<string?>("sessionStorage.getItem", "token");
    }

    public async Task RemoveToken()
    {
        await _js.InvokeVoidAsync("sessionStorage.removeItem", "token");
    }
}