using MediatR;
using Warehouse.Domain.Interfaces;

namespace Warehouse.Application.Queries.Suppliers.GetSupplierDocuments;

public class GetSupplierDocumentsHandler : IRequestHandler<GetSupplierDocumentsRequest, List<GetSupplierDocumentsResponse>>
{
    private readonly ISupplierDocumentRepository _supplierDocumentRepository;

    public GetSupplierDocumentsHandler(ISupplierDocumentRepository supplierDocumentRepository)
    {
        _supplierDocumentRepository = supplierDocumentRepository;
    }

    public async Task<List<GetSupplierDocumentsResponse>> Handle(GetSupplierDocumentsRequest request, CancellationToken cancellationToken)
    {
        var documents = await _supplierDocumentRepository.GetBySupplierIdAsync(request.SupplierId, cancellationToken);

        return documents
            .Select(document => new GetSupplierDocumentsResponse
            {
                Id = document.Id,
                FileName = document.FileName
            })
            .ToList();
    }
}