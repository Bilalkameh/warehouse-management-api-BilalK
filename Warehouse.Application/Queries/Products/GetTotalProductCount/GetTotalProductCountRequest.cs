using MediatR;

namespace Warehouse.Application.Queries.Products.GetTotalProductCount;

public class GetTotalProductCountRequest
    : IRequest<GetTotalProductCountResponse>
{
}