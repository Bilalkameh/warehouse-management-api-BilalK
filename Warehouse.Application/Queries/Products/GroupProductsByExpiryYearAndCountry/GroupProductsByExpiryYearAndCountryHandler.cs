using MediatR;
using Warehouse.Domain.Interfaces;

namespace Warehouse.Application.Queries.Products.GroupProductsByExpiryYearAndCountry;

public class GroupProductsByExpiryYearAndCountryHandler
    : IRequestHandler<
        GroupProductsByExpiryYearAndCountryRequest,
        List<GroupProductsByExpiryYearAndCountryResponse>>
{
    private readonly IProductRepository _repository;

    public GroupProductsByExpiryYearAndCountryHandler(IProductRepository repository)
    {
        _repository = repository;
    }

    public Task<List<GroupProductsByExpiryYearAndCountryResponse>> Handle(
        GroupProductsByExpiryYearAndCountryRequest request,
        CancellationToken cancellationToken)
    {
        var groups = _repository.GroupByExpiryYearAndCountry();
        var response =
            new List<GroupProductsByExpiryYearAndCountryResponse>();

        foreach (var group in groups)
        {
            response.Add(
                new GroupProductsByExpiryYearAndCountryResponse
                {
                    ExpiryYear = group.ExpiryYear,
                    Country = group.Country,
                    Count = group.Count
                });
        }

        return Task.FromResult(response);
    }
}