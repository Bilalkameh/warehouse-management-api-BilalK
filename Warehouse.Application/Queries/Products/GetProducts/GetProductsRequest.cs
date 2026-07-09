using MediatR;

namespace Warehouse.Application.Queries.Products.GetProducts;

public class GetProductsRequest : IRequest<List<GetProductsResponse>>
{
    public bool OnlyAvailable { get; set; }
}