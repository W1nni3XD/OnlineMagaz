namespace OnlineShop.Domain.Models.Entities;

public class Category
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    /// <summary>Если задано — категория создана этим продавцом; null — общая (админ).</summary>
    public int? SellerId { get; set; }
    public User? Seller { get; set; }

    public ICollection<Product> Products { get; set; } = new List<Product>();
}
