namespace OnlineShop.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Buyer")]
public class WishlistController : ControllerBase
{
    private readonly AppDbContext _context;

    public WishlistController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetWishlist()
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        var items = await _context.WishlistItems
            .Include(w => w.Product)
            .ThenInclude(p => p.Category)
            .Where(w => w.UserId == userId)
            .Select(w => new WishlistItemDto
            {
                Id = w.Id,
                ProductId = w.ProductId,
                ProductName = w.Product.Name,
                ProductPrice = w.Product.Price,
                ProductImageUrl = w.Product.ImageUrl,
                ProductStock = w.Product.Stock,
                CategoryName = w.Product.Category.Name,
                AddedAt = w.AddedAt
            }).ToListAsync();

        return Ok(items);
    }

    [HttpPost]
    public async Task<IActionResult> AddToWishlist(AddToWishlistDto dto)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        // Проверяем что товар существует
        var product = await _context.Products.FindAsync(dto.ProductId);
        if (product == null)
            return NotFound("Товар не найден");

        // Проверяем что товар ещё не в избранном
        var existing = await _context.WishlistItems
            .FirstOrDefaultAsync(w => w.UserId == userId && w.ProductId == dto.ProductId);

        if (existing != null)
            return BadRequest("Товар уже в избранном");

        _context.WishlistItems.Add(new WishlistItem
        {
            UserId = userId,
            ProductId = dto.ProductId
        });

        await _context.SaveChangesAsync();
        return Ok();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> RemoveFromWishlist(int id)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        var item = await _context.WishlistItems
            .FirstOrDefaultAsync(w => w.Id == id && w.UserId == userId);

        if (item == null) return NotFound();

        _context.WishlistItems.Remove(item);
        await _context.SaveChangesAsync();
        return Ok();
    }

    [HttpDelete("product/{productId}")]
    public async Task<IActionResult> RemoveByProductId(int productId)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        var item = await _context.WishlistItems
            .FirstOrDefaultAsync(w => w.UserId == userId && w.ProductId == productId);

        if (item == null) return NotFound();

        _context.WishlistItems.Remove(item);
        await _context.SaveChangesAsync();
        return Ok();
    }
}
