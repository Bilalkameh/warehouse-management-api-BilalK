using MediatR;
using Warehouse.Application.ViewModels;

namespace Warehouse.Application.Queries.Products.GetProducts;

public class GetProductsRequest : IRequest<List<ProductViewModel>>
{
    public bool OnlyAvailable { get; set; }
}