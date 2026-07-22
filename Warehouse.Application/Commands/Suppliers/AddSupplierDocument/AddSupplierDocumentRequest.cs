using MediatR;

namespace Warehouse.Application.Commands.Suppliers.AddSupplierDocument;

public class AddSupplierDocumentRequest : IRequest<AddSupplierDocumentResponse>
{
    public Guid SupplierId { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public long FileSize { get; set; }
    public Stream FileStream { get; set; } = Stream.Null;
}