namespace OnlineShop.API.Models.Entities;

public class CartItem
{
    public int Id { get; set; }
    public int Quantity { get; set; }

    public int UserId { get; set; }
    public User User { get; set; } = null!;

    public int ProductId { get; set; }
    public Product Product { get; set; } = null!;
}