using MediatR;

namespace Warehouse.Application.Queries.Products.GetProductImageUrl;

public class GetProductImageUrlRequest : IRequest<GetProductImageUrlResponse>
{
    public Guid ImageId { get; set; }
}