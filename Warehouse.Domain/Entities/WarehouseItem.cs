namespace Warehouse.Domain.Entities;

// We will consider this as sort of the inventory aspect of the product
// Product describes the actual product item and its characteristics like name and price...
// Warehouse Location shows additional information about the inventory/warehouse information of the product
public class WarehouseItem
{
    public Guid Id { get; private set; }
    public Guid ProductId { get; private set; }
    public string Location { get; private set; } = string.Empty;
    
    public WarehouseItem(
        Guid productId,
        int quantity,
        string location)
    {
        if (productId == Guid.Empty)
            throw new Exception("Product id is required.");

        if (string.IsNullOrWhiteSpace(location))
            throw new Exception("Location is required.");

        Id = Guid.NewGuid();
        ProductId = productId;
        Location = location;
    }
    
}