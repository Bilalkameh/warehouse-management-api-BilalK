namespace Warehouse.Domain.Entities;


// Product is a Domain entity
//It contains business rules related to product state and behavior

public class Product
{
    public Guid Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
// Changed SKU to required because the lab requires it
    public string SKU { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public double Price { get; private set; }
    public int QuantityInStock { get; private set; }
    public string SupplierName { get; private set; } = string.Empty;
    public DateTime ExpiryDate { get; private set; }
    public bool IsArchived { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime LastUpdatedAt { get; private set; }
    
public Product(
    string name,
    string sku,
    string description,
    double price,
    int quantityInStock,
    string supplierName,
    DateTime expiryDate)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new Exception("Product name is required.");

		if (string.IsNullOrWhiteSpace(sku))
    		throw new Exception("SKU is required.");
        
        if (string.IsNullOrWhiteSpace(supplierName))
            throw new Exception("Supplier name is required.");

        if (price <= 0)
            throw new Exception("Price must be greater than zero.");

        if (quantityInStock < 0)
            throw new Exception("Quantity cannot be negative.");
        
        Id = Guid.NewGuid();
        Name = name;
		SKU = sku;
        Description = description;
        Price = price;
        QuantityInStock = quantityInStock;
        SupplierName = supplierName;
        ExpiryDate = expiryDate;
        CreatedAt = DateTime.UtcNow;
        LastUpdatedAt = DateTime.UtcNow;
    }
	
	// Price updates are handled here instead of Application/Controllers because the rules belong to the Product entity itself.

    public void UpdatePrice(double newPrice)
    {
        if (IsArchived)
            throw new Exception("Archived products cannot be updated.");

        if (newPrice <= 0)
            throw new Exception("Price must be greater than zero.");
        Price = newPrice;
        LastUpdatedAt = DateTime.UtcNow;
    }

	// Same with price it belongs to the Product entity
    public void UpdateQuantity(int newQuantity)
    {
        if (IsArchived)
            throw new Exception("Archived products cannot be updated.");
        if (newQuantity < 0)
            throw new Exception("Quantity cannot be negative.");


        QuantityInStock = newQuantity;
        LastUpdatedAt = DateTime.UtcNow;
    }

	public void AssignSupplier(Supplier supplier)
	{
    	if (IsArchived)
        	throw new Exception("Archived products cannot be updated.");

    	if (supplier == null)
        	throw new ArgumentNullException(nameof(supplier));

    	if (!supplier.IsActive)
        	throw new Exception("Cannot assign inactive supplier.");

    	SupplierName = supplier.Name;
    	LastUpdatedAt = DateTime.UtcNow;
	}



    public void Archive()
    {
        if (IsArchived)
            return;
        IsArchived = true;
        LastUpdatedAt = DateTime.UtcNow;
    }
}