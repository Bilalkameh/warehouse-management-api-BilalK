using Minio;
using Minio.DataModel.Args;
using Warehouse.Application.Interfaces;

namespace Warehouse.Infrastructure.Storage;

public class MinioStorageService : IFileStorageService
{
    private readonly IMinioClient _minioClient;
    private readonly string _bucketName;

    public MinioStorageService(string endpoint, string accessKey, string secretKey, string bucketName, bool useSsl)
    {
        _bucketName = bucketName;
        
        _minioClient = new MinioClient()
            .WithEndpoint(endpoint)
            .WithCredentials(accessKey, secretKey)
            .WithSSL(useSsl)
            .Build();
    }

    public async Task UploadAsync(Stream fileStream, long fileSize, string objectKey, string contentType,
        CancellationToken cancellationToken)
    {
        var args = new PutObjectArgs()
            .WithBucket(_bucketName)
            .WithObject(objectKey)
            .WithStreamData(fileStream)
            .WithObjectSize(fileSize)
            .WithContentType(contentType);

        await _minioClient.PutObjectAsync(args, cancellationToken);
    }
    
    public async Task<byte[]> DownloadAsync(string objectKey, CancellationToken cancellationToken = default)
    {
        using var memoryStream = new MemoryStream();

        var args = new GetObjectArgs()
            .WithBucket(_bucketName)
            .WithObject(objectKey)
            .WithCallbackStream(stream =>
            {
                stream.CopyTo(memoryStream);
            });

        await _minioClient.GetObjectAsync(args, cancellationToken);

        return memoryStream.ToArray();
    }
    
    
    public async Task DeleteAsync(string objectKey, CancellationToken cancellationToken = default)
    {
        var args = new RemoveObjectArgs()
            .WithBucket(_bucketName)
            .WithObject(objectKey);

        await _minioClient.RemoveObjectAsync(args, cancellationToken);
    }
    
    public async Task<string> GetPresignedUrlAsync(string objectKey, int expirySeconds = 1000)
    {
        var args = new PresignedGetObjectArgs()
            .WithBucket(_bucketName)
            .WithObject(objectKey)
            .WithExpiry(expirySeconds);

        return await _minioClient.PresignedGetObjectAsync(args);
    }
}

