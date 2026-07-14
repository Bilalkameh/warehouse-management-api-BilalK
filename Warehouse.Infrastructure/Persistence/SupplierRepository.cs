using Microsoft.EntityFrameworkCore;
using Warehouse.Domain.Entities;
using Warehouse.Domain.Interfaces;

namespace Warehouse.Infrastructure.Persistence;

public class SupplierRepository : ISupplierRepository
{
    private readonly WarehouseDbContext _context;

    public SupplierRepository(WarehouseDbContext context)
    {
        _context = context;
    }

    public async Task<List<Supplier>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await _context.Suppliers
            .ToListAsync(cancellationToken);
    }

    public async Task<Supplier?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _context.Suppliers
            .FirstOrDefaultAsync(
                supplier => supplier.SupplierId == id,
                cancellationToken);
    }

    public async Task<Supplier?> GetByNameAsync(string name, CancellationToken cancellationToken)
    {
        var lowerName = name.ToLower();

        return await _context.Suppliers
            .FirstOrDefaultAsync(
                supplier =>
                    supplier.Name.ToLower() == lowerName, cancellationToken);
    }

    public async Task AddAsync(Supplier supplier, CancellationToken cancellationToken)
    {
        await _context.Suppliers.AddAsync(supplier, cancellationToken);

        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Supplier supplier, CancellationToken cancellationToken)
    {
        _context.Suppliers.Update(supplier);
        
        await _context.SaveChangesAsync(cancellationToken);
    }
}