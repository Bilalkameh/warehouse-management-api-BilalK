using MediatR;
using Warehouse.Application.Exceptions;
using Warehouse.Application.Interfaces;
using Warehouse.Domain.Entities;
using Warehouse.Domain.Interfaces;

namespace Warehouse.Application.Commands.Suppliers.AddSupplierDocument;

public class AddSupplierDocumentHandler : IRequestHandler<AddSupplierDocumentRequest, AddSupplierDocumentResponse>
{
    private readonly ISupplierRepository _supplierRepository;
    private readonly ISupplierDocumentRepository _supplierDocumentRepository;
    private readonly IFileStorageService _fileStorageService;

    public AddSupplierDocumentHandler(ISupplierRepository supplierRepository, ISupplierDocumentRepository supplierDocumentRepository, IFileStorageService fileStorageService)
    {
        _supplierRepository = supplierRepository;
        _supplierDocumentRepository = supplierDocumentRepository;
        _fileStorageService = fileStorageService;
    }

    public async Task<AddSupplierDocumentResponse> Handle(
        AddSupplierDocumentRequest request,
        CancellationToken cancellationToken)
    {
        var supplier = await _supplierRepository.GetByIdAsync(request.SupplierId, cancellationToken);

        if (supplier == null)
            throw new NotFoundException("Supplier was not found.");

        var extension = Path.GetExtension(request.FileName);

        var filePath = $"suppliers/{supplier.SupplierId}/{Guid.NewGuid()}{extension}";

        await _fileStorageService.UploadAsync(request.FileStream, request.FileSize, filePath, request.ContentType, cancellationToken);

        var document = new SupplierDocument(supplier, request.FileName, filePath);

        await _supplierDocumentRepository.AddAsync(document, cancellationToken);

        return new AddSupplierDocumentResponse
        {
            Id = document.Id,
            Success = true
        };
    }
}