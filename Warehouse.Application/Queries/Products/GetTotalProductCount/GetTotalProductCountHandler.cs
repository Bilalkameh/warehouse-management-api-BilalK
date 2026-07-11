using MediatR;
using Warehouse.Domain.Interfaces;

namespace Warehouse.Application.Queries.Products.GetTotalProductCount;

public class GetTotalProductCountHandler
    : IRequestHandler<GetTotalProductCountRequest, GetTotalProductCountResponse>
{
    private readonly IProductRepository _repository;

    public GetTotalProductCountHandler(IProductRepository repository)
    {
        _repository = repository;
    }

    public Task<GetTotalProductCountResponse> Handle(
        GetTotalProductCountRequest request,
        CancellationToken cancellationToken)
    {
        var total = _repository.GetTotalCount();
        var response = new GetTotalProductCountResponse();
        response.Total = total;

        return Task.FromResult(response);
    }
}