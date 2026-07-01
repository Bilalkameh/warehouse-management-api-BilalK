using System.ComponentModel.DataAnnotations;

namespace warehouse_management.Models;

public class Product
{
    [Required]
    [StringLength(5,MinimumLength = 1)]
    public string Id { get; set; }
    
    [Required]
    [StringLength(40)]
    public string Name { get; set; }
    
    [Required]
    [StringLength(25)]
    public string SKU;
    
    
    public string Description;
    
    [Required]
    public double Price { get; set; }
    
    public int QuantityInStock { get; set; }
    public string SupplierName { get; set; }
    public DateTime ExpiryDate { get; set; }
    public bool IsArchived { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime LastUpdatedAt { get; set; }
}