namespace OnlineShop.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IWebHostEnvironment _env;
    private readonly ILogger<ProductsController> _logger;

    public ProductsController(AppDbContext context, IWebHostEnvironment env, ILogger<ProductsController> logger)
    {
        _context = context;
        _env = env;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] int? categoryId,
        [FromQuery] string? search,
        [FromQuery] decimal? minPrice,
        [FromQuery] decimal? maxPrice,
        [FromQuery] bool? inStock,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 12)
    {
        var query = _context.Products
            .Include(p => p.Category)
            .Include(p => p.Seller)
            .AsQueryable();

        if (categoryId.HasValue)
            query = query.Where(p => p.CategoryId == categoryId.Value);
        if (!string.IsNullOrEmpty(search))
            query = query.Where(p => p.Name.ToLower().Contains(search.ToLower())
                                  || p.Description.ToLower().Contains(search.ToLower()));
        if (minPrice.HasValue)
            query = query.Where(p => p.Price >= minPrice.Value);
        if (maxPrice.HasValue)
            query = query.Where(p => p.Price <= maxPrice.Value);
        if (inStock.HasValue && inStock.Value)
            query = query.Where(p => p.Stock > 0);

        var totalCount = await query.CountAsync();
        var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

        var products = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(p => new ProductDto
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                Price = p.Price,
                Stock = p.Stock,
                ImageUrl = p.ImageUrl,
                CategoryId = p.CategoryId,
                CategoryName = p.Category.Name,
                SellerId = p.SellerId,
                SellerEmail = p.Seller.Email
            }).ToListAsync();

        return Ok(new { Products = products, TotalCount = totalCount, TotalPages = totalPages, CurrentPage = page, PageSize = pageSize });
    }

    [HttpGet("mine")]
    [Authorize(Roles = "Seller,Admin")]
    public async Task<IActionResult> GetMine()
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var role = User.FindFirstValue(ClaimTypes.Role)!;

        var query = _context.Products.Include(p => p.Category).Include(p => p.Seller).AsQueryable();

        if (!string.Equals(role, "Admin", StringComparison.OrdinalIgnoreCase))
            query = query.Where(p => p.SellerId == userId);

        var products = await query.OrderByDescending(p => p.Id)
            .Select(p => new ProductDto
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                Price = p.Price,
                Stock = p.Stock,
                ImageUrl = p.ImageUrl,
                CategoryId = p.CategoryId,
                CategoryName = p.Category.Name,
                SellerId = p.SellerId,
                SellerEmail = p.Seller.Email
            }).ToListAsync();

        return Ok(products);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var product = await _context.Products
            .Include(p => p.Category).Include(p => p.Seller)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (product == null) return NotFound();

        return Ok(new ProductDto
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            Price = product.Price,
            Stock = product.Stock,
            ImageUrl = product.ImageUrl,
            CategoryId = product.CategoryId,
            CategoryName = product.Category.Name,
            SellerId = product.SellerId,
            SellerEmail = product.Seller.Email
        });
    }

    [HttpPost]
    [Authorize(Roles = "Seller,Admin")]
    public async Task<IActionResult> Create(CreateProductDto dto)
    {
        var sellerId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        var product = new Product
        {
            Name = dto.Name,
            Description = dto.Description,
            Price = dto.Price,
            Stock = dto.Stock,
            ImageUrl = dto.ImageUrl ?? string.Empty,
            CategoryId = dto.CategoryId,
            SellerId = sellerId
        };

        _context.Products.Add(product);
        await _context.SaveChangesAsync();
        _logger.LogInformation("Товар создан: {Name}", product.Name);
        return Ok(product.Id);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Seller,Admin")]
    public async Task<IActionResult> Update(int id, CreateProductDto dto)
    {
        var product = await _context.Products.FindAsync(id);
        if (product == null) return NotFound();

        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var userRole = User.FindFirst(ClaimTypes.Role)!.Value;

        if (userRole != "Admin" && product.SellerId != userId) return Forbid();

        product.Name = dto.Name; product.Description = dto.Description;
        product.Price = dto.Price; product.Stock = dto.Stock;
        product.ImageUrl = dto.ImageUrl ?? string.Empty;
        product.CategoryId = dto.CategoryId;

        await _context.SaveChangesAsync();
        _logger.LogInformation("Товар обновлён: {Id}", id);
        return Ok();
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Seller,Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var product = await _context.Products.FindAsync(id);
        if (product == null) return NotFound();

        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var userRole = User.FindFirst(ClaimTypes.Role)!.Value;

        if (userRole != "Admin" && product.SellerId != userId) return Forbid();

        _context.Products.Remove(product);
        await _context.SaveChangesAsync();
        _logger.LogInformation("Товар удалён: {Id}", id);
        return Ok();
    }

    [HttpPost("upload-image")]
    [Authorize(Roles = "Seller,Admin")]
    public async Task<IActionResult> UploadImage(
        [FromServices] ImageService imageService,
        IFormFile file)
    {
        try
        {
            if (file == null || file.Length == 0)
                return BadRequest("Файл не выбран");

            var url = await imageService.SaveImageAsync(file);
            _logger.LogInformation("Картинка загружена: {Url}", url);
            return Ok(new { ImageUrl = url });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка загрузки картинки");
            return StatusCode(500, ex.Message);
        }
    }
}