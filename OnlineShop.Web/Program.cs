using MudBlazor.Services;
using Microsoft.AspNetCore.Components.Authorization;
using OnlineShop.Web.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents(options =>
    {
        options.JSInteropDefaultCallTimeout = TimeSpan.FromMinutes(5);
    });

var apiBaseUrl = builder.Configuration["ApiBaseUrl"] ?? "http://localhost:5170/";

builder.Services.AddHttpClient("API", client =>
{
    client.BaseAddress = new Uri(apiBaseUrl);
});

builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<ProductService>();
builder.Services.AddScoped<CartService>();
builder.Services.AddScoped<CartSidebarState>();
builder.Services.AddScoped<OrderService>();
builder.Services.AddScoped<CategoryService>();
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<WishlistService>();

builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthStateProvider>();
builder.Services.AddAuthorizationCore();
builder.Services.AddAuthentication();
builder.Services.AddAuthorization();
builder.Services.AddMudServices();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
}

app.Use(async (context, next) =>
{
    if (context.Request.Path.StartsWithSegments("/images"))
    {
        var client = context.RequestServices.GetRequiredService<IHttpClientFactory>().CreateClient("API");
        var targetPath = context.Request.Path.Value!.TrimStart('/') + context.Request.QueryString;
        using var request = new HttpRequestMessage(HttpMethod.Get, targetPath);
        using var response = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, context.RequestAborted);

        context.Response.StatusCode = (int)response.StatusCode;
        if (response.Content.Headers.ContentType is { } contentType)
            context.Response.ContentType = contentType.ToString();

        await response.Content.CopyToAsync(context.Response.Body, context.RequestAborted);
        return;
    }

    await next();
});

app.UseStaticFiles();
app.UseAuthentication();
app.UseAuthorization();
app.UseAntiforgery();

app.MapRazorComponents<OnlineShop.Web.Components.App>()
    .AddInteractiveServerRenderMode()
    .DisableAntiforgery();

app.Run();