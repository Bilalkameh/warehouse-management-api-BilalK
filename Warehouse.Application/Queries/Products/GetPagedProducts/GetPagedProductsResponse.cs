namespace Warehouse.Application.Queries.Products.GetPagedProducts;

public class GetPagedProductsResponse
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string SKU { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public double Price { get; set; }

    public int QuantityInStock { get; set; }

    public string SupplierName { get; set; } = string.Empty;

    public DateTime ExpiryDate { get; set; }

    public DateTime CreatedAt { get; set; }
}