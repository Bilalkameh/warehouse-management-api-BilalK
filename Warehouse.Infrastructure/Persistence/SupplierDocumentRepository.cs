using Microsoft.EntityFrameworkCore;
using Warehouse.Domain.Entities;
using Warehouse.Domain.Interfaces;

namespace Warehouse.Infrastructure.Persistence;

public class SupplierDocumentRepository : ISupplierDocumentRepository
{
    private readonly WarehouseDbContext _context;

    public SupplierDocumentRepository(WarehouseDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(SupplierDocument document, CancellationToken cancellationToken)
    {
        await _context.SupplierDocuments.AddAsync(document, cancellationToken);

        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<List<SupplierDocument>> GetBySupplierIdAsync(Guid supplierId, CancellationToken cancellationToken)
    {
        return await _context.SupplierDocuments
            .Where(document => document.SupplierId == supplierId)
            .ToListAsync(cancellationToken);
    }

    public async Task<SupplierDocument?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _context.SupplierDocuments
            .FirstOrDefaultAsync(document => document.Id == id, cancellationToken);
    }

    public async Task DeleteAsync(SupplierDocument document,CancellationToken cancellationToken)
    {
        _context.SupplierDocuments.Remove(document);

        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task ReplaceAsync(SupplierDocument oldDocument, SupplierDocument newDocument, CancellationToken cancellationToken)
    {
        _context.SupplierDocuments.Remove(oldDocument);

        await _context.SupplierDocuments.AddAsync(newDocument, cancellationToken);

        await _context.SaveChangesAsync(cancellationToken);
    }
}