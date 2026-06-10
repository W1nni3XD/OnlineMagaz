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

    public async Task<string> SaveImageAsync(IFormFile file)
    {
        var imagesFolder = Path.Combine(_env.ContentRootPath, "Images");
        if (!Directory.Exists(imagesFolder))
            Directory.CreateDirectory(imagesFolder);

        var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
        var filePath = Path.Combine(imagesFolder, fileName);

        using var stream = new FileStream(filePath, FileMode.Create);
        await file.CopyToAsync(stream);

        _logger.LogInformation("Изображение сохранено: {FileName}", fileName);
        return $"http://localhost:5170/images/{fileName}";
    }
}