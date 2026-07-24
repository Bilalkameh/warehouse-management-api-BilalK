namespace Warehouse.Application.Queries.Suppliers.GetSupplierDocumentUrl;

public class GetSupplierDocumentUrlResponse
{
    public string Url { get; set; } = string.Empty;
    public int ExpiresInSeconds { get; set; }
}