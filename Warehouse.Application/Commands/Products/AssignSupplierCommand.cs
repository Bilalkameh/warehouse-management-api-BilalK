namespace Warehouse.Application.Commands.Products;

public class AssignSupplierCommand
{
    public Guid ProductId { get; init; }
    public Guid SupplierId { get; init; }
}