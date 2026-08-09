using Warehouse.Application.Interfaces;

namespace Warehouse.Api.IntegrationTests.TestServices;

public class TestFileStorageService : IFileStorageService
{
private readonly Dictionary<string, StoredFile> _files = new();

    public IReadOnlyDictionary<string, StoredFile> Files => _files;

    public async Task UploadAsync(
        Stream fileStream,
        long fileSize,
        string objectKey,
        string contentType,
        CancellationToken cancellationToken)
    {
        using var memoryStream = new MemoryStream();

        await fileStream.CopyToAsync(
            memoryStream,
            cancellationToken);

        _files[objectKey] = new StoredFile(
            memoryStream.ToArray(),
            fileSize,
            contentType);
    }

    public Task<byte[]> DownloadAsync(
        string objectKey,
        CancellationToken cancellationToken)
    {
        return Task.FromResult(_files[objectKey].Content);
    }

    public Task DeleteAsync(
        string objectKey,
        CancellationToken cancellationToken)
    {
        _files.Remove(objectKey);
        return Task.CompletedTask;
    }

    public Task<string> GetPresignedUrlAsync(
        string objectKey,
        int expirySeconds = 1000)
    {
        return Task.FromResult(
            $"https://test-storage.local/{objectKey}");
    }

    public void Reset()
    {
        _files.Clear();
    }

    public record StoredFile(
        byte[] Content,
        long FileSize,
        string ContentType);
}