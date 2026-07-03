using System.ComponentModel.DataAnnotations;

namespace warehouse_management.Contracts;

public class CreateProductRequest
{
    [Required(ErrorMessage = "Product name required.")]
    [StringLength(40, MinimumLength = 1, ErrorMessage = "Product name must be between 1 and 40 characters.")]
    public string Name { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "SKU required.")]
    [StringLength(25, ErrorMessage = "SKU cannot exceed 25 characters.")]
    public string SKU { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "Product must have a description.")]
    [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters.")]
    public string Description { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "Product must have a price.")]
    [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than zero.")]
    public double Price { get; set; }
    
    
    [Required(ErrorMessage = "Quantity is required.")]
    [Range(0, int.MaxValue, ErrorMessage = "Quantity cannot be negative.")]
    public int QuantityInStock { get; set; }
    
    [Required(ErrorMessage = "Supplier name is required.")]
    public string SupplierName { get; set; } = string.Empty;
    
    public DateTime ExpiryDate { get; set; }
}

