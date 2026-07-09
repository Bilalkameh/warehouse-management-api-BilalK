using MediatR;

namespace Warehouse.Application.Commands.Products.ArchiveProduct;

public class ArchiveProductRequest : IRequest<ArchiveProductResponse>
{
    public Guid ProductId { get; set; }
}