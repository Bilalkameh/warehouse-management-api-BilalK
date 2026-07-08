namespace Warehouse.Application.Commands.Products;

public class UpdateProductQuantityCommand
{
    public Guid ProductId { get; init; }
    public int Quantity { get; init; }
}