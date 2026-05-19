namespace OnlineShop.Domain.DTOs;

public class UserProfileDto
{
    public string Email { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public string? SellerDisplayName { get; set; }
}

public class ChangePasswordDto
{
    public string CurrentPassword { get; set; } = string.Empty;
    public string NewPassword { get; set; } = string.Empty;
}
