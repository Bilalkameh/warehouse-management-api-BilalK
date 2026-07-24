namespace Warehouse.Application.Queries.Products.GetProductImages;

public class GetProductImagesResponse
{
    public Guid Id { get; set; }
    public string FileName { get; set; } = string.Empty;
}