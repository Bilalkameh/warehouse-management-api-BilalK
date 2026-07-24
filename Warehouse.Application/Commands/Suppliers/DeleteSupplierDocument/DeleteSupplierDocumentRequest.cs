using MediatR;

namespace Warehouse.Application.Commands.Suppliers.DeleteSupplierDocument;

public class DeleteSupplierDocumentRequest : IRequest
{
    public Guid DocumentId { get; set; }
}