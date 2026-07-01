using System.ComponentModel.DataAnnotations;

namespace warehouse_management.Contracts;

public class UpdateProductQuantityRequest
{
    [Required]
    public int QuantityInStock { get; set; }
}