using MediatR;

namespace Warehouse.Application.Commands.Products.AssignSupplier;

public class AssignSupplierRequest : IRequest<AssignSupplierResponse>
{
    public Guid ProductId { get;set; }
    public Guid SupplierId { get; set; }
}