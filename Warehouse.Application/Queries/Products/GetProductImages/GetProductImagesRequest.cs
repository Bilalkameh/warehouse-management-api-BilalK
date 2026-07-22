using MediatR;

namespace Warehouse.Application.Queries.Products.GetProductImages;

public class GetProductImagesRequest : IRequest<List<GetProductImagesResponse>>
{
    public Guid ProductId { get; set; }
}