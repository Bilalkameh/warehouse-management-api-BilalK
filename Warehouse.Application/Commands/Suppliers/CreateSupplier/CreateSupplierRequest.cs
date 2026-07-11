using MediatR;
using Warehouse.Application.ViewModels;

namespace Warehouse.Application.Commands.Suppliers.CreateSupplier;

public class CreateSupplierRequest : IRequest<SupplierViewModel>
{
    public string Name { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public string ContactEmail { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
}