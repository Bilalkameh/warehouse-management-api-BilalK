using MediatR;
using Warehouse.Application.ViewModels;

namespace Warehouse.Application.Queries.Suppliers.GetSupplierById;

public class GetSupplierByIdRequest : IRequest<SupplierViewModel?>
{
    public Guid SupplierId { get; set; }
}