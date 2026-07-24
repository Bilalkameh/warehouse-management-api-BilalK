using MediatR;
using Warehouse.Application.Exceptions;
using Warehouse.Application.Interfaces;
using Warehouse.Domain.Interfaces;

namespace Warehouse.Application.Queries.Suppliers.GetSupplierDocumentUrl;

public class GetSupplierDocumentUrlHandler : IRequestHandler<GetSupplierDocumentUrlRequest, GetSupplierDocumentUrlResponse>
{
    private readonly ISupplierDocumentRepository _supplierDocumentRepository;
    private readonly IFileStorageService _fileStorageService;

    public GetSupplierDocumentUrlHandler(ISupplierDocumentRepository supplierDocumentRepository, IFileStorageService fileStorageService)
    {
        _supplierDocumentRepository = supplierDocumentRepository;
        _fileStorageService = fileStorageService;
    }

    public async Task<GetSupplierDocumentUrlResponse> Handle(GetSupplierDocumentUrlRequest request, CancellationToken cancellationToken)
    {
        var document =
            await _supplierDocumentRepository.GetByIdAsync(request.DocumentId, cancellationToken);

        if (document == null)
            throw new NotFoundException("Supplier document was not found.");

        const int expirySeconds = 1000;

        var url = await _fileStorageService.GetPresignedUrlAsync(document.FilePath, expirySeconds);

        return new GetSupplierDocumentUrlResponse
        {
            Url = url,
            ExpiresInSeconds = expirySeconds
        };
    }
}