namespace OnlineShop.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UserController : ControllerBase
{
    private readonly AppDbContext _context;

    public UserController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet("profile")]
    public async Task<IActionResult> GetProfile()
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var user = await _context.Users.FindAsync(userId);

        if (user == null)
            return NotFound();

        return Ok(new UserProfileDto
        {
            Email = user.Email,
            Role = user.Role,
            SellerDisplayName = user.DisplayName
        });
    }

    [HttpPut("change-password")]
    public async Task<IActionResult> ChangePassword(ChangePasswordDto dto)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var user = await _context.Users.FindAsync(userId);

        if (user == null)
            return NotFound();

        // Проверяем текущий пароль
        if (!BCrypt.Net.BCrypt.Verify(dto.CurrentPassword, user.PasswordHash))
        {
            return BadRequest("Неверный текущий пароль");
        }

        // Валидация нового пароля
        if (string.IsNullOrWhiteSpace(dto.NewPassword) || dto.NewPassword.Length < 6)
        {
            return BadRequest("Новый пароль должен содержать минимум 6 символов");
        }

        // Обновляем пароль
        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);
        await _context.SaveChangesAsync();

        return Ok();
    }
}
