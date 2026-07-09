using MediatR;

namespace Warehouse.Application.Queries.Products.GetProductById;

public class GetProductByIdRequest : IRequest<GetProductByIdResponse?>
{
    public Guid ProductId { get; set; }
}