namespace Warehouse.Application.Commands.Products;

public class AddProductImageCommand
{
    public Guid ProductId { get; init; }

    public string FileName { get; init; } = string.Empty;

    public string FilePath { get; init; } = string.Empty;
}