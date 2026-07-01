using System.ComponentModel.DataAnnotations;

namespace warehouse_management.Models;

public class ProductImage
{
    [Required]
    [StringLength(50,MinimumLength = 1)]
    public int ProductId { get; set; }
    
    [Required]
    [StringLength(40)]
    public string FileName { get; set; }
    
    [Required]
    [StringLength(150)]
    public string FilePath { get; set; }

}