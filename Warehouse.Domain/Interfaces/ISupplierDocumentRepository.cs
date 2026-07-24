using Warehouse.Domain.Entities;

namespace Warehouse.Domain.Interfaces;

public interface ISupplierDocumentRepository
{
    Task AddAsync(SupplierDocument document, CancellationToken cancellationToken);

    Task<List<SupplierDocument>> GetBySupplierIdAsync(Guid supplierId, CancellationToken cancellationToken);

    Task<SupplierDocument?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    Task DeleteAsync(SupplierDocument document, CancellationToken cancellationToken);

    Task ReplaceAsync(SupplierDocument oldDocument, SupplierDocument newDocument, CancellationToken cancellationToken);
}