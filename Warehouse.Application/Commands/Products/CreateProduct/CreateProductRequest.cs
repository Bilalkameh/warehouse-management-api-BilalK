using MediatR;
using Warehouse.Application.ViewModels;

namespace Warehouse.Application.Commands.Products.CreateProduct;

public class CreateProductRequest : IRequest<ProductViewModel>
{
    public string Name { get; set; } = string.Empty;
    public string SKU { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public double Price { get; set; }
    public int QuantityInStock { get; set; }
    public string SupplierName { get; set; } = string.Empty;
    public DateTime ExpiryDate { get; set; }
}