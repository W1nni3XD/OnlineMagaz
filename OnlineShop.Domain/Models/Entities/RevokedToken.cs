namespace OnlineShop.Domain.Models.Entities;

public class RevokedToken
{
    public int Id { get; set; }
    public string Jti { get; set; } = string.Empty; // уникадльный id токен
    public DateTime ExpiresAt { get; set; }          // когда токен и так истечёт
    public DateTime RevokedAt { get; set; } = DateTime.UtcNow;
}