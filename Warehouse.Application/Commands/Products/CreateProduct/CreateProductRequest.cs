using System.ComponentModel.DataAnnotations;
using MediatR;
using Warehouse.Application.ViewModels;
using Warehouse.Application.Validation;

namespace Warehouse.Application.Commands.Products.CreateProduct;

public class CreateProductRequest : IRequest<ProductViewModel>
{
    [Required(ErrorMessage = "Product name is required.")]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "SKU is required.")]
    [StringLength(50)]
    public string SKU { get; set; } = string.Empty;

    [StringLength(500)]
    public string Description { get; set; } = string.Empty;

    [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than zero.")]
    public double Price { get; set; }

    [Range(0, int.MaxValue, ErrorMessage = "Quantity cannot be negative.")]
    public int QuantityInStock { get; set; }

    [Required(ErrorMessage = "Supplier name is required.")]
    [StringLength(100)]
    public string SupplierName { get; set; } = string.Empty;

    [FutureDate]
    public DateTime ExpiryDate { get; set; }
}