namespace Warehouse.Application.Queries.Suppliers.DownloadSupplierDocument;

public class DownloadSupplierDocumentResponse
{
    public string FileName { get; set; } = string.Empty;
    public byte[] FileData { get; set; } = [];
}