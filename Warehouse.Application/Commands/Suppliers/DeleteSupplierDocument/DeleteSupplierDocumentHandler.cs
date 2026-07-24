using MediatR;
using Warehouse.Application.Exceptions;
using Warehouse.Application.Interfaces;
using Warehouse.Domain.Interfaces;

namespace Warehouse.Application.Commands.Suppliers.DeleteSupplierDocument;

public class DeleteSupplierDocumentHandler : IRequestHandler<DeleteSupplierDocumentRequest>
{
    private readonly ISupplierDocumentRepository _supplierDocumentRepository;
    private readonly IFileStorageService _fileStorageService;

    public DeleteSupplierDocumentHandler(ISupplierDocumentRepository supplierDocumentRepository, IFileStorageService fileStorageService)
    {
        _supplierDocumentRepository = supplierDocumentRepository;
        _fileStorageService = fileStorageService;
    }

    public async Task Handle(DeleteSupplierDocumentRequest request, CancellationToken cancellationToken)
    {
        var document =
            await _supplierDocumentRepository.GetByIdAsync(request.DocumentId, cancellationToken);

        if (document == null)
            throw new NotFoundException("Supplier document was not found.");

        await _fileStorageService.DeleteAsync(document.FilePath, cancellationToken);

        await _supplierDocumentRepository.DeleteAsync(document, cancellationToken);
    }
}