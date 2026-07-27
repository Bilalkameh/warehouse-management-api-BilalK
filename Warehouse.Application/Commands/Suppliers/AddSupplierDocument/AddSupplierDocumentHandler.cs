using MediatR;
using Warehouse.Application.Exceptions;
using Warehouse.Application.IntegrationEvents;
using Warehouse.Application.Interfaces;
using Warehouse.Domain.Entities;
using Warehouse.Domain.Interfaces;

namespace Warehouse.Application.Commands.Suppliers.AddSupplierDocument;

public class AddSupplierDocumentHandler : IRequestHandler<AddSupplierDocumentRequest, AddSupplierDocumentResponse>
{
    private readonly ISupplierRepository _supplierRepository;
    private readonly ISupplierDocumentRepository _supplierDocumentRepository;
    private readonly IFileStorageService _fileStorageService;
    private readonly IWarehouseEventPublisher _eventPublisher;
    private readonly ICorrelationIdAccessor _correlationIdAccessor;

    public AddSupplierDocumentHandler(ISupplierRepository supplierRepository, ISupplierDocumentRepository supplierDocumentRepository, 
        IFileStorageService fileStorageService, IWarehouseEventPublisher eventPublisher, ICorrelationIdAccessor correlationIdAccessor)
    {
        _supplierRepository = supplierRepository;
        _supplierDocumentRepository = supplierDocumentRepository;
        _fileStorageService = fileStorageService;
        _eventPublisher = eventPublisher;
        _correlationIdAccessor = correlationIdAccessor;
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

        var fileUploadedEvent = new WarehouseFileUploaded
        {
            CorrelationId = _correlationIdAccessor.CorrelationId,
            RelatedEntityId = document.Id,
            SupplierId = document.SupplierId,
            FileName = document.FileName,
            FilePath = document.FilePath,
            ContentType = request.ContentType,
            FileSize = request.FileSize
        };

        await _eventPublisher.PublishAsync(fileUploadedEvent, WarehouseEventRoutingKeys.FileUploaded, cancellationToken);

        return new AddSupplierDocumentResponse
        {
            Id = document.Id,
            Success = true
        };
    }
}