using MediatR;

namespace Warehouse.Application.Queries.Products.SearchProducts;

public class SearchProductsRequest : IRequest<List<SearchProductsResponse>>
{
    public string? Name { get; set; }
    public string? SupplierName { get; set; }
}