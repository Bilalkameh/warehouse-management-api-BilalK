using MediatR;

namespace Warehouse.Application.Commands.Products.DeleteProductImage;

public class DeleteProductImageRequest : IRequest
{
    public Guid ImageId { get; set; }
}