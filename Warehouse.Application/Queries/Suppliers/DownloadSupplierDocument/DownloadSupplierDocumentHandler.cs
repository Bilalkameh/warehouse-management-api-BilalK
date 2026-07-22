using MediatR;
using Warehouse.Application.Exceptions;
using Warehouse.Application.Interfaces;
using Warehouse.Domain.Interfaces;

namespace Warehouse.Application.Queries.Suppliers.DownloadSupplierDocument;

public class DownloadSupplierDocumentHandler : IRequestHandler<DownloadSupplierDocumentRequest, DownloadSupplierDocumentResponse>
{
    private readonly ISupplierDocumentRepository _supplierDocumentRepository;
    private readonly IFileStorageService _fileStorageService;

    public DownloadSupplierDocumentHandler(ISupplierDocumentRepository supplierDocumentRepository, IFileStorageService fileStorageService)
    {
        _supplierDocumentRepository = supplierDocumentRepository;
        _fileStorageService = fileStorageService;
    }

    public async Task<DownloadSupplierDocumentResponse> Handle(DownloadSupplierDocumentRequest request, CancellationToken cancellationToken)
    {
        var document =
            await _supplierDocumentRepository.GetByIdAsync(request.DocumentId, cancellationToken);

        if (document == null)
            throw new NotFoundException("Supplier document was not found.");

        var fileData = await _fileStorageService.DownloadAsync(document.FilePath, cancellationToken);

        return new DownloadSupplierDocumentResponse
        {
            FileName = document.FileName,
            FileData = fileData
        };
    }
}