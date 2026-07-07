using System.ComponentModel.DataAnnotations;

namespace warehouse_management.Contracts;

public class UpdateProductQuantityRequest
{
    [Required(ErrorMessage = "Quantity required.")]
    [Range(0, int.MaxValue, ErrorMessage = "Quantity cannot be negative.")]
    public int QuantityInStock { get; set; }
}