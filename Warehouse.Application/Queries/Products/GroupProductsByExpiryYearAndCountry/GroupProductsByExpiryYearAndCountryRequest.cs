using MediatR;

namespace Warehouse.Application.Queries.Products.GroupProductsByExpiryYearAndCountry;

public class GroupProductsByExpiryYearAndCountryRequest
    : IRequest<List<GroupProductsByExpiryYearAndCountryResponse>>
{
    //same for all these grouping endpoints we don't need any request params
}