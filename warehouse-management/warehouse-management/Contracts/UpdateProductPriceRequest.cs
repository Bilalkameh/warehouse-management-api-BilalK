using System.ComponentModel.DataAnnotations;

namespace warehouse_management.Contracts;

public class UpdateProductPriceRequest
{
    [Required]
    public double Price { get; set; }
}