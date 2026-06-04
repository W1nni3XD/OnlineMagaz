namespace OnlineShop.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Buyer")]
public class CartController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly ILogger<CartController> _logger;

    public CartController(AppDbContext context, ILogger<CartController> logger)
    {
        _context = context;
        _logger = logger;
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
                Quantity = c.Quantity,
                ProductStock = c.Product.Stock
            }).ToListAsync();

        return Ok(items);
    }

    [HttpPost]
    public async Task<IActionResult> AddToCart(AddToCartDto dto)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        var product = await _context.Products.FindAsync(dto.ProductId);
        if (product == null)
            return NotFound("Товар не найден");

        var existing = await _context.CartItems
            .FirstOrDefaultAsync(c => c.UserId == userId && c.ProductId == dto.ProductId);

        int newQuantity = dto.Quantity;
        if (existing != null)
            newQuantity = existing.Quantity + dto.Quantity;

        if (newQuantity > product.Stock)
            return BadRequest($"Недостаточно товара на складе. Доступно: {product.Stock} шт.");

        if (existing != null)
            existing.Quantity = newQuantity;
        else
            _context.CartItems.Add(new CartItem { UserId = userId, ProductId = dto.ProductId, Quantity = dto.Quantity });

        await _context.SaveChangesAsync();
        _logger.LogInformation("Товар {ProductId} добавлен в корзину пользователя {UserId}", dto.ProductId, userId);
        return Ok();
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateQuantity(int id, UpdateCartItemDto dto)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        var item = await _context.CartItems
            .Include(c => c.Product)
            .FirstOrDefaultAsync(c => c.Id == id && c.UserId == userId);

        if (item == null) return NotFound();

        if (dto.Quantity > item.Product.Stock)
            return BadRequest($"Недостаточно товара на складе. Доступно: {item.Product.Stock} шт.");

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

        var items = await _context.CartItems.Where(c => c.UserId == userId).ToListAsync();
        _context.CartItems.RemoveRange(items);
        await _context.SaveChangesAsync();
        _logger.LogInformation("Корзина очищена для пользователя {UserId}", userId);
        return Ok();
    }
}