using MediatR;

namespace Warehouse.Application.Commands.Suppliers.DeactivateSupplier;

public class DeactivateSupplierRequest : IRequest<DeactivateSupplierResponse>
{
    public Guid SupplierId { get; set; }
}