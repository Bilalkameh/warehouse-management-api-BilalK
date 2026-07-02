using System.ComponentModel.DataAnnotations;

namespace warehouse_management.Models;

public class Product
{
    [Required]
    [StringLength(50,MinimumLength = 1)]
    public string Id { get; set; }
    
    [Required]
    [StringLength(40)]
    public string Name { get; set; }
    
    
    [StringLength(25)]
    public string? SKU { get; set; }
    
    
    public string Description { get; set; }
    
    [Required]
    public double Price { get; set; }
    
    public int QuantityInStock { get; set; }

    [Required]
    public string SupplierName { get; set; }
    public DateTime ExpiryDate { get; set; }
    public bool IsArchived { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime LastUpdatedAt { get; set; }
    
    // Constructor with all the fields except CreatedAt and LastUpdated
    // createdAt and lastUpdatedAt are assigned automatically using the current time of creation
    // This avoids allowing dates that don't make sense and reflects the actual creation time of the product
    public Product(string id, string name, string description, double price, int quantityInStock, string supplierName, DateTime expiryDate)
    {
        this.Id = id;
        this.Name = name;
        this.Description = description;
        this.Price = price;
        this.QuantityInStock = quantityInStock;
        this.SupplierName = supplierName;
        this.ExpiryDate = expiryDate;
        CreatedAt = DateTime.UtcNow;
        LastUpdatedAt = DateTime.UtcNow;
    }
    
    //Added a default constructor with no parameters
    public Product()
    {
        
    }
}