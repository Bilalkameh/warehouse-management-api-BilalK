using MediatR;

namespace Warehouse.Application.Queries.Products.DownloadProductImage;

public class DownloadProductImageRequest : IRequest<DownloadProductImageResponse>
{
    public Guid ImageId { get; set; }
}