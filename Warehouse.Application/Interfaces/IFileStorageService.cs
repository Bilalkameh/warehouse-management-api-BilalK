namespace Warehouse.Application.Interfaces;

public interface IFileStorageService
{
    Task UploadAsync(Stream fileStream, long fileSize, string objectKey, string contentType, CancellationToken cancellationToken);

    Task<byte[]> DownloadAsync(string objectKey, CancellationToken cancellationToken);

    Task DeleteAsync(string objectKey, CancellationToken cancellationToken);

    Task<string> GetPresignedUrlAsync(string objectKey, int expirySeconds = 1000);
}