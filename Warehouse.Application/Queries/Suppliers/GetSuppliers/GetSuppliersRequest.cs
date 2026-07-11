using MediatR;
using Warehouse.Application.ViewModels;

namespace Warehouse.Application.Queries.Suppliers.GetSuppliers;

public class GetSuppliersRequest : IRequest<List<SupplierViewModel>>
{
    // since we are getting all we do not need any fields in the request
}