using System.ComponentModel.DataAnnotations;

namespace warehouse_management.Models;

public class Supplier
{
    [Required(ErrorMessage = "Id required.")]
    [StringLength(50, MinimumLength = 1, ErrorMessage = "Id must be between 1 and 50 characters.")]
    public string Id { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "Supplier name required.")]
    [StringLength(40, MinimumLength = 1, ErrorMessage = "Supplier name must be between 1 and 40 characters.")]
    public string Name { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "Country required.")]
    [StringLength(40, MinimumLength = 1, ErrorMessage = "Country must be between 1 and 40 characters.")]
    public string Country { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "Contact email required.")]
    [EmailAddress(ErrorMessage = "Invalid email format.")]
    [StringLength(100, ErrorMessage = "Contact email cannot exceed 100 characters.")]
    public string ContactEmail { get; set; } = string.Empty;

    [Required(ErrorMessage = "Phone number required.")]
    [Phone(ErrorMessage = "Invalid phone number format.")]
    [StringLength(25, ErrorMessage = "Phone number cannot exceed 25 characters.")]
    public string PhoneNumber { get; set; } = string.Empty;
    
    public bool IsActive { get; set; } = true;

    public Supplier(string id, string name, string country, string contactEmail, string phoneNumber)
    {
        Id = id;
        Name = name;
        Country = country;
        ContactEmail = contactEmail;
        PhoneNumber = phoneNumber;
        IsActive = true;
    }

    public Supplier()
    {
        
    }



}