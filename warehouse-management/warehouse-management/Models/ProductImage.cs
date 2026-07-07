using System.ComponentModel.DataAnnotations;

namespace warehouse_management.Models;

public class ProductImage
{   
    [Required(ErrorMessage = "Product Id required.")]
    [StringLength(50, MinimumLength = 1, ErrorMessage = "Product Id must be between 1 and 50 characters.")]
    // I had mistakenly made ProductId an int 
    public string ProductId { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "File name required.")]
    [StringLength(40, ErrorMessage = "File name cannot exceed 40 characters.")]
    public string FileName { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "File path is required.")]
    [StringLength(150, ErrorMessage = "File path cannot exceed 150 characters.")]
    public string FilePath { get; set; } = string.Empty;
    
}