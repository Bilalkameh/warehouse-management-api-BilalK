using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Warehouse.Application.Interfaces;
using Warehouse.Infrastructure.Storage;

namespace Warehouse.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddMinioStorage(this IServiceCollection services, IConfiguration configuration)
    {
        var endpoint = configuration["Minio:Endpoint"]
                       ?? throw new InvalidOperationException("MinIO Endpoint is not configured.");

        var accessKey = configuration["Minio:AccessKey"]
                        ?? throw new InvalidOperationException("MinIO AccessKey is not configured.");

        var secretKey = configuration["Minio:SecretKey"]
                        ?? throw new InvalidOperationException("MinIO SecretKey is not configured.");

        var bucketName = configuration["Minio:BucketName"]
                         ?? throw new InvalidOperationException("MinIO BucketName is not configured.");

        var useSsl = bool.TryParse(configuration["Minio:UseSSL"], out var parsedUseSsl) && parsedUseSsl;

        services.AddSingleton<IFileStorageService>(_ => new MinioStorageService(endpoint, accessKey, secretKey, bucketName, useSsl));

        return services;
    }
}