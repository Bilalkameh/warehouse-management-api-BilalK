using MediatR;

namespace Warehouse.Application.Queries.Suppliers.DownloadSupplierDocument;

public class DownloadSupplierDocumentRequest : IRequest<DownloadSupplierDocumentResponse>
{
    public Guid DocumentId { get; set; }
}