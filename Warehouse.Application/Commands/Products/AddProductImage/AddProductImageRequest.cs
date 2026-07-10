using MediatR;

namespace Warehouse.Application.Commands.Products.AddProductImage;

public class AddProductImageRequest : IRequest<AddProductImageResponse>
{
    public Guid ProductId { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string FilePath { get; set; } = string.Empty;
}