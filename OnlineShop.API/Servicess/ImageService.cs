namespace OnlineShop.API.Services;

public class ImageService
{
    private readonly IWebHostEnvironment _env;
    private readonly ILogger<ImageService> _logger;

    public ImageService(IWebHostEnvironment env, ILogger<ImageService> logger)
    {
        _env = env;
        _logger = logger;
    }

    private static readonly string[] AllowedExtensions = { ".jpg", ".jpeg", ".png", ".gif", ".webp", ".bmp" };
    private const long MaxFileSize = 10 * 1024 * 1024;

    public async Task<string> SaveImageAsync(IFormFile file)
    {
        if (file == null || file.Length == 0)
            throw new InvalidOperationException("Файл не выбран");

        if (file.Length > MaxFileSize)
            throw new InvalidOperationException("Размер файла не должен превышать 10 МБ");

        var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (!AllowedExtensions.Contains(ext))
            throw new InvalidOperationException($"Недопустимый формат файла. Разрешены: {string.Join(", ", AllowedExtensions)}");

        var imagesPath = Path.Combine(_env.ContentRootPath, "Images");
        if (!Directory.Exists(imagesPath))
            Directory.CreateDirectory(imagesPath);

        var fileName = $"{Guid.NewGuid()}{ext}";
        var filePath = Path.Combine(imagesPath, fileName);

        await using var stream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None, 4096, FileOptions.Asynchronous);
        await file.CopyToAsync(stream);

        _logger.LogInformation("Изображение сохранено: {FileName}", fileName);
        return $"http://localhost:5170/images/{fileName}";
    }
}