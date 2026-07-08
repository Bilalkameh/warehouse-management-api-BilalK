namespace Warehouse.Application.Commands.Products;

public class UpdateProductPriceCommand
{
    public Guid ProductId { get; init; }
    public double Price { get; init; }
}