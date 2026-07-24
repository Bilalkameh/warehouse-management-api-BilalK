namespace Warehouse.Application.Queries.Products.GetProductImageUrl;

public class GetProductImageUrlResponse
{
    public string Url { get; set; } = string.Empty;
    public int ExpiresInSeconds { get; set; }
}