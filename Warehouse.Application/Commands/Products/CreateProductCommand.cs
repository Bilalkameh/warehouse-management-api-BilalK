namespace Warehouse.Application.Commands.Products;


//Here we replace the setter from {get; set} with init because a command represents a request
//We do not want a request to be modifiable so we remove the setter and once it is created it cannot be changed.
public class CreateProductCommand
{ 
    public string Name { get; init; } = string.Empty;
    public string SKU { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public double Price { get; init; }
    public int QuantityInStock { get; init; }
    public string SupplierName { get; init; } = string.Empty;
    public DateTime ExpiryDate { get; init; }
}