using MediatR;

namespace Warehouse.Application.Queries.Suppliers.GetSuppliers;

public class GetSuppliersRequest : IRequest<List<GetSuppliersResponse>>
{
    // since we are getting all we do not need any fields in the request
}