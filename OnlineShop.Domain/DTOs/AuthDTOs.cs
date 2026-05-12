namespace OnlineShop.Domain.DTOs;

public class RegisterDto
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string Role { get; set; } = "Buyer";
    /// <summary>Обязательно при Role = Seller — имя продавца.</summary>
    public string? SellerDisplayName { get; set; }
}

public class LoginDto
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public class AuthResponseDto
{
    public string Token { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    /// <summary>Текст для профиля в UI: имя продавца или email покупателя.</summary>
    public string ProfileDisplay { get; set; } = string.Empty;
}