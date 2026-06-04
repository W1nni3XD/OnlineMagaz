namespace OnlineShop.API;

public static class ServiceExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<MinioService>();
        services.AddScoped<TokenService>();
        services.AddScoped<EmailService>();
        return services;
    }
}