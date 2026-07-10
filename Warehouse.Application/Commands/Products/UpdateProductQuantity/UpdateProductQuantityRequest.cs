using MediatR;

namespace Warehouse.Application.Commands.Products.UpdateProductQuantity;

public class UpdateProductQuantityRequest : IRequest<UpdateProductQuantityResponse>
{
    public Guid ProductId { get; set; }
    public int Quantity { get; set; }
}