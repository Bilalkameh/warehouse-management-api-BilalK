using System.ComponentModel.DataAnnotations;

namespace warehouse_management.Contracts;

public class CreateProductRequest
{
    [Required]
    [StringLength(40)]
    public string Name { get; set; }
    
    [Required]
    [StringLength(25)]
    public string SKU { get; set; }
    
    [Required]
    public string Description { get; set; }
    
    [Required]
    public double Price { get; set; }
    
    
    [Required]
    public int QuantityInStock { get; set; }
    
    [Required]
    public string SupplierName { get; set; }
    
    public DateTime ExpiryDate { get; set; }
}

