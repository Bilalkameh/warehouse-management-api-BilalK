using System.ComponentModel.DataAnnotations;

namespace warehouse_management.Models;

public class Product
{
    [Required(ErrorMessage = "Id required")]
    [StringLength(50,MinimumLength = 1, ErrorMessage = "Id must be between 1 and 50 characters." )]
    public string Id { get; set; }
    
    [Required(ErrorMessage = "Product name required.")]
    [StringLength(40, MinimumLength = 1,ErrorMessage = "Product name must be between 1 and 40 characters.")]
    public string Name { get; set; }
    
    
    [StringLength(25, ErrorMessage = "SKU cannot exceed 25 characters.")]
    public string? SKU { get; set; }
    
    [Required(ErrorMessage = "Product must have a description.")]
    [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters.")]
    public string Description { get; set; }
    
    [Required(ErrorMessage = "Product must have a price.")]
    [Range(0.01, double.MaxValue,ErrorMessage = "Price must be greater than zero.")]
    public double Price { get; set; }
    
    [Required(ErrorMessage = "Quantity required.")]
    [Range(0, int.MaxValue,ErrorMessage = "Quantity cannot be negative.")]
    public int QuantityInStock { get; set; }

    [Required(ErrorMessage = "Supplier name required.")]
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