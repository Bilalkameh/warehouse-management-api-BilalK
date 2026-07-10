using MediatR;

namespace Warehouse.Application.Queries.Suppliers.GetSupplierById;

public class GetSupplierByIdRequest : IRequest<GetSupplierByIdResponse?>
{
    public Guid SupplierId { get; set; }
}