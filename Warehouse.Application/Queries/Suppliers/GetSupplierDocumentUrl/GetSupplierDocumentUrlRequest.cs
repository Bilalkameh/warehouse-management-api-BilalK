using MediatR;

namespace Warehouse.Application.Queries.Suppliers.GetSupplierDocumentUrl;

public class GetSupplierDocumentUrlRequest : IRequest<GetSupplierDocumentUrlResponse>
{
    public Guid DocumentId { get; set; }
}