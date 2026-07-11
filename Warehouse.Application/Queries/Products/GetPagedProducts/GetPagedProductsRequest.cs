using MediatR;

namespace Warehouse.Application.Queries.Products.GetPagedProducts;

public class GetPagedProductsRequest
    : IRequest<List<GetPagedProductsResponse>>
{
    public int PageSize { get; set; }

    public int PageNumber { get; set; }
}