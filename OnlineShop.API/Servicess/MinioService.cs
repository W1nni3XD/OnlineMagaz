using Minio;
using Minio.DataModel.Args;

namespace OnlineShop.API.Services;

public class MinioService
{
    private readonly IMinioClient _minio;
    private readonly string _bucket;
    private readonly ILogger<MinioService> _logger;

    public MinioService(IConfiguration configuration, ILogger<MinioService> logger)
    {
        _logger = logger;
        var s = configuration.GetSection("MinioSettings");
        _minio = new MinioClient()
            .WithEndpoint(s["Endpoint"])
            .WithCredentials(s["AccessKey"], s["SecretKey"])
            .Build();
        _bucket = s["BucketName"]!;
    }

    public async Task<string> UploadImageAsync(IFormFile file)
    {
        try
        {
            var exists = await _minio.BucketExistsAsync(new BucketExistsArgs().WithBucket(_bucket));
            if (!exists)
                await _minio.MakeBucketAsync(new MakeBucketArgs().WithBucket(_bucket));

            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";

            using var stream = file.OpenReadStream();
            await _minio.PutObjectAsync(new PutObjectArgs()
                .WithBucket(_bucket)
                .WithObject(fileName)
                .WithStreamData(stream)
                .WithObjectSize(file.Length)
                .WithContentType(file.ContentType));

            return $"http://localhost:9000/{_bucket}/{fileName}";
        }
        catch (Exception ex)
        {
            // Console.WriteLine($"MinIO ERROR: {ex.GetType().Name}: {ex.Message}");
            // Console.WriteLine(ex.StackTrace);
            _logger.LogError(ex, "Ошибка при сохранении картинки");
            throw;
        }
    }
}