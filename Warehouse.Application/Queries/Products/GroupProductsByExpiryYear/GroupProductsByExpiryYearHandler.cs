using MediatR;
using Warehouse.Domain.Interfaces;

namespace Warehouse.Application.Queries.Products.GroupProductsByExpiryYear;

public class GroupProductsByExpiryYearHandler
    : IRequestHandler<GroupProductsByExpiryYearRequest, List<GroupProductsByExpiryYearResponse>>
{
    private readonly IProductRepository _repository;

    public GroupProductsByExpiryYearHandler(IProductRepository repository)
    {
        _repository = repository;
    }

    public Task<List<GroupProductsByExpiryYearResponse>> Handle(
        GroupProductsByExpiryYearRequest request,
        CancellationToken cancellationToken)
    {
        var groups = _repository.GroupByExpiryYear();
        var response = new List<GroupProductsByExpiryYearResponse>();

        foreach (var group in groups)
        {
            response.Add(new GroupProductsByExpiryYearResponse
            {
                ExpiryYear = group.ExpiryYear,
                Count = group.Count
            });
        }
        return Task.FromResult(response);
    }
}