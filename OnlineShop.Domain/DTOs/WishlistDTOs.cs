namespace OnlineShop.Domain.DTOs;

public class WishlistItemDto
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public decimal ProductPrice { get; set; }
    public string ProductImageUrl { get; set; } = string.Empty;
    public int ProductStock { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public DateTime AddedAt { get; set; }
}

public class AddToWishlistDto
{
    public int ProductId { get; set; }
}
