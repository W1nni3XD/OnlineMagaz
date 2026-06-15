using Microsoft.AspNetCore.Components.Forms;

namespace OnlineShop.Web.Services;

public class ProductService : BaseApiService
{
    public ProductService(IHttpClientFactory httpClientFactory, AuthService authService)
        : base(httpClientFactory, authService) { }

    public async Task<PaginatedProductsDto> GetAll(
        int? categoryId = null,
        string? search = null,
        decimal? minPrice = null,
        decimal? maxPrice = null,
        bool? inStock = null,
        int page = 1,
        int pageSize = 12)
    {
        var client = await GetClient();
        var url = "api/products";
        var query = new List<string>();
        if (categoryId.HasValue) query.Add($"categoryId={categoryId}");
        if (!string.IsNullOrEmpty(search)) query.Add($"search={Uri.EscapeDataString(search)}");
        if (minPrice.HasValue) query.Add($"minPrice={minPrice}");
        if (maxPrice.HasValue) query.Add($"maxPrice={maxPrice}");
        if (inStock.HasValue) query.Add($"inStock={inStock.Value.ToString().ToLower()}");
        query.Add($"page={page}");
        query.Add($"pageSize={pageSize}");
        if (query.Any()) url += "?" + string.Join("&", query);
        var result = await client.GetFromJsonAsync<PaginatedProductsDto>(url);
        return result ?? new PaginatedProductsDto();
    }

    public async Task<ProductDto?> GetById(int id)
    {
        var client = await GetClient();
        return await client.GetFromJsonAsync<ProductDto>($"api/products/{id}");
    }

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

    public async Task<string?> UploadImage(IBrowserFile file)
    {
        var client = await GetClient(true);
        var content = new MultipartFormDataContent();
        var stream = file.OpenReadStream(maxAllowedSize: 10 * 1024 * 1024);
        content.Add(new StreamContent(stream), "file", file.Name);
        var response = await client.PostAsync("api/products/upload-image", content);
        if (!response.IsSuccessStatusCode) return null;
        var result = await response.Content.ReadFromJsonAsync<UploadResult>();
        return result?.ImageUrl;
    }

    private record UploadResult(string ImageUrl);
}