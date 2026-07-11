using MediatR;

namespace Warehouse.Application.Queries.Products.GroupProductsByExpiryYear;

public class GroupProductsByExpiryYearRequest : IRequest<List<GroupProductsByExpiryYearResponse>>
{
    //we dont need anything in the request because we are going to get all the products then group them
}