using System.ComponentModel.DataAnnotations;

namespace warehouse_management.Contracts;

public class UpdateProductPriceRequest
{
    [Required(ErrorMessage = "Product must have a price.")]
    [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than zero.")]
    public double Price { get; set; }
}