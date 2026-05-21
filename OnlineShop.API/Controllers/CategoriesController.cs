namespace OnlineShop.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoriesController : ControllerBase
{
    private readonly AppDbContext _context;

    public CategoriesController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var categories = await _context.Categories
            .OrderBy(c => c.Name)
            .Select(c => new CategoryDto
            {
                Id = c.Id,
                Name = c.Name,
                Description = c.Description
            }).ToListAsync();

        return Ok(categories);
    }

    /// <summary>Категории для выбора в товаре: все категории доступны продавцу</summary>
    [HttpGet("available")]
    [Authorize(Roles = "Seller,Admin")]
    public async Task<IActionResult> GetAvailableForSeller()
    {
        var categories = await _context.Categories
            .OrderBy(c => c.Name)
            .Select(c => new CategoryDto
            {
                Id = c.Id,
                Name = c.Name,
                Description = c.Description
            }).ToListAsync();

        return Ok(categories);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create(CategoryDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Name))
            return BadRequest("Укажите название категории");

        var category = new Category
        {
            Name = dto.Name.Trim(),
            Description = dto.Description?.Trim() ?? string.Empty
        };

        _context.Categories.Add(category);
        await _context.SaveChangesAsync();
        return Ok(category.Id);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(int id, CategoryDto dto)
    {
        var category = await _context.Categories.FindAsync(id);
        if (category == null) return NotFound();

        if (string.IsNullOrWhiteSpace(dto.Name))
            return BadRequest("Укажите название категории");

        category.Name = dto.Name.Trim();
        category.Description = dto.Description?.Trim() ?? string.Empty;

        await _context.SaveChangesAsync();
        return Ok();
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var category = await _context.Categories.FindAsync(id);
        if (category == null) return NotFound();

        var hasProducts = await _context.Products.AnyAsync(p => p.CategoryId == id);
        if (hasProducts)
            return BadRequest("Нельзя удалить категорию, пока к ней привязаны товары.");

        _context.Categories.Remove(category);
        await _context.SaveChangesAsync();
        return Ok();
    }
}
