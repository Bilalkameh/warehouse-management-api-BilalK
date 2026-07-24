using MediatR;
using Warehouse.Application.Exceptions;
using Warehouse.Application.Interfaces;
using Warehouse.Domain.Entities;
using Warehouse.Domain.Interfaces;

namespace Warehouse.Application.Commands.Suppliers.ReplaceSupplierDocument;

public class ReplaceSupplierDocumentHandler : IRequestHandler<ReplaceSupplierDocumentRequest, ReplaceSupplierDocumentResponse>
{
    private readonly ISupplierRepository _supplierRepository;
    private readonly ISupplierDocumentRepository _supplierDocumentRepository;
    private readonly IFileStorageService _fileStorageService;

    public ReplaceSupplierDocumentHandler(ISupplierRepository supplierRepository, ISupplierDocumentRepository supplierDocumentRepository, IFileStorageService fileStorageService)
    {
        _supplierRepository = supplierRepository;
        _supplierDocumentRepository = supplierDocumentRepository;
        _fileStorageService = fileStorageService;
    }

    public async Task<ReplaceSupplierDocumentResponse> Handle(ReplaceSupplierDocumentRequest request, CancellationToken cancellationToken)
    {
        var oldDocument = await _supplierDocumentRepository.GetByIdAsync(request.DocumentId, cancellationToken);

        if (oldDocument == null)
            throw new NotFoundException("Supplier document was not found.");

        var supplier = await _supplierRepository.GetByIdAsync(oldDocument.SupplierId, cancellationToken);

        if (supplier == null)
            throw new NotFoundException("Supplier was not found.");

        var extension = Path.GetExtension(request.FileName);

        var newFilePath = $"suppliers/{oldDocument.SupplierId}/{Guid.NewGuid()}{extension}";

        await _fileStorageService.UploadAsync(request.FileStream, request.FileSize, newFilePath, request.ContentType, cancellationToken);

        var newDocument = new SupplierDocument(supplier, request.FileName, newFilePath);

        await _supplierDocumentRepository.ReplaceAsync(oldDocument, newDocument, cancellationToken);

        await _fileStorageService.DeleteAsync(oldDocument.FilePath, cancellationToken);

        return new ReplaceSupplierDocumentResponse
        {
            Id = newDocument.Id,
            Success = true
        };
    }
}