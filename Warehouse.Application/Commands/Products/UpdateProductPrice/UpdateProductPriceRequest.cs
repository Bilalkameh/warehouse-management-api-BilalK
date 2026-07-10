using MediatR;
namespace Warehouse.Application.Commands.Products.UpdateProductPrice;

public class UpdateProductPriceRequest : IRequest<UpdateProductPriceResponse>
{
    public Guid ProductId { get; set; }
    public double Price { get; set; }
}