using MediatR;

namespace Warehouse.Application.Commands.Products.ReplaceProductImage;

public class ReplaceProductImageRequest : IRequest<ReplaceProductImageResponse>
{
    public Guid ImageId { get; set; }

    public string FileName { get; set; } = string.Empty;

    public string ContentType { get; set; } = string.Empty;

    public long FileSize { get; set; }

    public Stream FileStream { get; set; } = Stream.Null;
}