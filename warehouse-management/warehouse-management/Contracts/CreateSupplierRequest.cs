using System.ComponentModel.DataAnnotations;

namespace warehouse_management.Contracts;

public class CreateSupplierRequest
{
    [Required(ErrorMessage = "Supplier name required.")]
    [StringLength(40, MinimumLength = 1, ErrorMessage = "Supplier name must be between 1 and 40 characters.")]
    public string Name { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "Country required.")]
    [StringLength(40, MinimumLength = 1, ErrorMessage = "Country must be between 1 and 40 characters.")]
    public string Country { get; set; } = string.Empty;

    [Required(ErrorMessage = "Contact email required.")]
    [EmailAddress(ErrorMessage = "Invalid email format.")]
    [StringLength(100, ErrorMessage = "Email cannot exceed 100 characters.")]
    public string ContactEmail { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "Phone number required.")]
    [StringLength(25, MinimumLength = 5, ErrorMessage = "Phone number must be between 5 and 25 characters.")]
    public string PhoneNumber { get; set; } = string.Empty;


    
}