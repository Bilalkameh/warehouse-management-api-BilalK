using Warehouse.Domain.Exceptions;

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
    
    public Guid SupplierId { get; private set; }

    public Supplier Supplier { get; private set; } = null!;
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
    Supplier supplier,
    DateTime expiryDate)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new BusinessRuleException("Product name is required.");

		if (string.IsNullOrWhiteSpace(sku))
    		throw new BusinessRuleException("SKU is required.");

        if (price <= 0)
            throw new BusinessRuleException("Price must be greater than zero.");

        if (quantityInStock < 0)
            throw new BusinessRuleException("Quantity cannot be negative.");
        
        if (supplier == null)
            throw new BusinessRuleException(nameof(supplier));
        
        Id = Guid.NewGuid();
        Name = name;
		SKU = sku;
        Description = description;
        Price = price;
        QuantityInStock = quantityInStock;
        SupplierId = supplier.SupplierId;
        Supplier = supplier;
        ExpiryDate = expiryDate;
        CreatedAt = DateTime.UtcNow;
        LastUpdatedAt = DateTime.UtcNow;
    }

    // parameterless constructor
    private Product()
    {
    }
	
	// Price updates are handled here instead of Application/Controllers because the rules belong to the Product entity itself.

    public void UpdatePrice(double newPrice)
    {
        if (IsArchived)
            throw new BusinessRuleException("Archived products cannot be updated.");

        if (newPrice <= 0)
            throw new BusinessRuleException("Price must be greater than zero.");
        Price = newPrice;
        LastUpdatedAt = DateTime.UtcNow;
    }

	// Same with price it belongs to the Product entity
    public void UpdateQuantity(int newQuantity)
    {
        if (IsArchived)
            throw new BusinessRuleException("Archived products cannot be updated.");
        if (newQuantity < 0)
            throw new BusinessRuleException("Quantity cannot be negative.");


        QuantityInStock = newQuantity;
        LastUpdatedAt = DateTime.UtcNow;
    }

	public void AssignSupplier(Supplier supplier)
	{
    	if (IsArchived)
        	throw new BusinessRuleException("Archived products cannot be updated.");

    	if (supplier == null)
        	throw new BusinessRuleException(nameof(supplier));

    	if (!supplier.IsActive)
        	throw new BusinessRuleException("Cannot assign inactive supplier.");

        Supplier = supplier;
        SupplierId = supplier.SupplierId;
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