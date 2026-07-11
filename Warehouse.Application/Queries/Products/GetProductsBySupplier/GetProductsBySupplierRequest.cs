using MediatR;

namespace Warehouse.Application.Queries.Products.GetProductsBySupplier;

public class GetProductsBySupplierRequest : IRequest<List<GetProductsBySupplierResponse>>
{
    public string SupplierName { get; set; } = string.Empty;
    public bool Ascending { get; set; }
}