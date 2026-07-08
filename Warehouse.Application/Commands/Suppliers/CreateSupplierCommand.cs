namespace Warehouse.Application.Commands.Suppliers;

public class CreateSupplierCommand
{
    public string Name { get; init; } = string.Empty;
    public string Country { get; init; } = string.Empty;
    public string ContactEmail { get; init; } = string.Empty;
    public string PhoneNumber { get; init; } = string.Empty;
}