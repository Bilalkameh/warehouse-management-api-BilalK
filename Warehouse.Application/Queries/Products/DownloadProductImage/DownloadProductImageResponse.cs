namespace Warehouse.Application.Queries.Products.DownloadProductImage;

public class DownloadProductImageResponse
{
    public string FileName { get; set; } = string.Empty;
    public byte[] FileData { get; set; } = [];
}