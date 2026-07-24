using MediatR;

namespace Warehouse.Application.Commands.Suppliers.ReplaceSupplierDocument;

public class ReplaceSupplierDocumentRequest : IRequest<ReplaceSupplierDocumentResponse>
{
    public Guid DocumentId { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public long FileSize { get; set; }
    public Stream FileStream { get; set; } = Stream.Null;
}