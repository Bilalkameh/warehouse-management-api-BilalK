using System.ComponentModel.DataAnnotations;
using MediatR;
using Warehouse.Application.ViewModels;

namespace Warehouse.Application.Commands.Suppliers.CreateSupplier;

public class CreateSupplierRequest : IRequest<SupplierViewModel>
{
    [Required(ErrorMessage = "Supplier name is required.")]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Supplier country is required.")]
    [StringLength(50)]
    public string Country { get; set; } = string.Empty;

    [Required(ErrorMessage = "Supplier email is required.")]
    [EmailAddress(ErrorMessage = "Invalid email format.")]
    [StringLength(100)]
    public string ContactEmail { get; set; } = string.Empty;

    [Required(ErrorMessage = "Supplier phone number is required.")]
    [StringLength(50)]
    public string PhoneNumber { get; set; } = string.Empty;
}