using System.IdentityModel.Tokens.Jwt;

namespace OnlineShop.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly TokenService _tokenService;
    private readonly EmailService _emailService;

    public AuthController(AppDbContext context, TokenService tokenService, EmailService emailService)
    {
        _context = context;
        _tokenService = tokenService;
        _emailService = emailService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterDto dto)
    {
        if (await _context.Users.AnyAsync(u => u.Email == dto.Email))
            return BadRequest("Пользователь с таким email уже существует");

        var isSeller = string.Equals(dto.Role, "Seller", StringComparison.OrdinalIgnoreCase);
        if (isSeller && string.IsNullOrWhiteSpace(dto.SellerDisplayName))
            return BadRequest("Укажите имя продавца");

        var user = new User
        {
            Email = dto.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
            Role = dto.Role,
            DisplayName = isSeller ? dto.SellerDisplayName!.Trim() : null
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        // ждём отправку письма 
        _ = Task.Run(() => _emailService.SendWelcomeEmailAsync(user.Email, user.Role));

        var token = _tokenService.GenerateToken(user);

        return Ok(new AuthResponseDto
        {
            Token = token,
            Email = user.Email,
            Role = user.Role,
            ProfileDisplay = TokenService.GetProfileDisplay(user)
        });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDto dto)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);

        if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
            return Unauthorized("Неверный email или пароль");

        var token = _tokenService.GenerateToken(user);

        return Ok(new AuthResponseDto
        {
            Token = token,
            Email = user.Email,
            Role = user.Role,
            ProfileDisplay = TokenService.GetProfileDisplay(user)
        });
    }

    [HttpPost("logout")]
    [Authorize]
    public async Task<IActionResult> Logout()
    {
        var authHeader = Request.Headers["Authorization"].ToString();
        var tokenStr = authHeader.Replace("Bearer ", "");

        var handler = new JwtSecurityTokenHandler();
        var jwtToken = handler.ReadJwtToken(tokenStr);

        var jti = jwtToken.Id;
        var exp = jwtToken.ValidTo;

        // записываем токен в блеклист
        _context.RevokedTokens.Add(new RevokedToken
        {
            Jti = jti,
            ExpiresAt = exp
        });
        await _context.SaveChangesAsync();

        // чистим старые истёкшие токены из бд (раз в логаут)
        var now = DateTime.UtcNow;
        var expired = _context.RevokedTokens.Where(r => r.ExpiresAt < now);
        _context.RevokedTokens.RemoveRange(expired);
        await _context.SaveChangesAsync();

        return Ok();
    }
}