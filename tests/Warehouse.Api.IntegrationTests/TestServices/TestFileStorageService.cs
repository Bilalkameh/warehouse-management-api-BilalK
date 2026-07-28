using Warehouse.Application.Interfaces;

namespace Warehouse.Api.IntegrationTests.TestServices;

public class TestFileStorageService : IFileStorageService
{
    public Task UploadAsync(Stream fileStream, long fileSize, string objectKey, string contentType, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    public Task<byte[]> DownloadAsync(string objectKey, CancellationToken cancellationToken)
    {
        return Task.FromResult(Array.Empty<byte>());
    }

    public Task DeleteAsync(string objectKey, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    public Task<string> GetPresignedUrlAsync(string objectKey, int expirySeconds = 1000)
    {
        return Task.FromResult($"https://test-storage/{objectKey}");
    }
}