using MailKit;

namespace OnlineShop.API;

public static class ServiceExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<TokenService>();
        services.AddScoped<EmailService>();
        return services;
    }
}