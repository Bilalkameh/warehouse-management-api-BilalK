using MediatR;

namespace Warehouse.Application.Queries.Suppliers.GetSupplierDocuments;

public class GetSupplierDocumentsRequest : IRequest<List<GetSupplierDocumentsResponse>>
{
    public Guid SupplierId { get; set; }
}