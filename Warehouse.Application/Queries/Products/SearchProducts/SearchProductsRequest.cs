using MediatR;
using Warehouse.Application.ViewModels;

namespace Warehouse.Application.Queries.Products.SearchProducts;

public class SearchProductsRequest : IRequest<List<ProductViewModel>>
{
    public string? Name { get; set; }
    public string? SupplierName { get; set; }
}