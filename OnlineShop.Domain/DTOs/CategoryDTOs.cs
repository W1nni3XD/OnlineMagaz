namespace OnlineShop.Domain.DTOs;

public class CategoryDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    /// <summary>Владелец категории (продавец); null — общая категория.</summary>
    public int? SellerId { get; set; }
}
