namespace OnlineShop.Web.Services;

public abstract class BaseApiService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly AuthService _authService;

    protected BaseApiService(IHttpClientFactory httpClientFactory, AuthService authService)
    {
        _httpClientFactory = httpClientFactory;
        _authService = authService;
    }

    protected async Task<HttpClient> GetClient(bool withAuth = false)
    {
        var client = _httpClientFactory.CreateClient("API");
        if (withAuth)
        {
            var token = await _authService.GetToken();
            if (!string.IsNullOrEmpty(token))
                client.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        }
        return client;
    }
}