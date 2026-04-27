namespace OnlineShop.API.Models.Entities;

public class User
{
    public int Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<Product> Products { get; set; } = new List<Product>();
    public ICollection<Order> Orders { get; set; } = new List<Order>();
    public ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();
}