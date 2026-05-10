namespace OnlineShop.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Buyer")]
public class CartController : ControllerBase
{
    private readonly AppDbContext _context;

    public CartController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetCart()
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        var items = await _context.CartItems
            .Include(c => c.Product)
            .Where(c => c.UserId == userId)
            .Select(c => new CartItemDto
            {
                Id = c.Id,
                ProductId = c.ProductId,
                ProductName = c.Product.Name,
                ProductPrice = c.Product.Price,
                ImageUrl = c.Product.ImageUrl,
                Quantity = c.Quantity
            }).ToListAsync();

        return Ok(items);
    }

    [HttpPost]
    public async Task<IActionResult> AddToCart(AddToCartDto dto)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        var existing = await _context.CartItems
            .FirstOrDefaultAsync(c => c.UserId == userId && c.ProductId == dto.ProductId);

        if (existing != null)
        {
            existing.Quantity += dto.Quantity;
        }
        else
        {
            _context.CartItems.Add(new CartItem
            {
                UserId = userId,
                ProductId = dto.ProductId,
                Quantity = dto.Quantity
            });
        }

        await _context.SaveChangesAsync();
        return Ok();
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateQuantity(int id, UpdateCartItemDto dto)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        var item = await _context.CartItems
            .FirstOrDefaultAsync(c => c.Id == id && c.UserId == userId);

        if (item == null) return NotFound();

        item.Quantity = dto.Quantity;
        await _context.SaveChangesAsync();
        return Ok();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> RemoveItem(int id)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        var item = await _context.CartItems
            .FirstOrDefaultAsync(c => c.Id == id && c.UserId == userId);

        if (item == null) return NotFound();

        _context.CartItems.Remove(item);
        await _context.SaveChangesAsync();
        return Ok();
    }

    [HttpDelete]
    public async Task<IActionResult> ClearCart()
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        var items = await _context.CartItems
            .Where(c => c.UserId == userId)
            .ToListAsync();

        _context.CartItems.RemoveRange(items);
        await _context.SaveChangesAsync();
        return Ok();
    }
}