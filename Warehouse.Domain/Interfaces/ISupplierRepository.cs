using Warehouse.Domain.Entities;

namespace Warehouse.Domain.Interfaces;


public interface ISupplierRepository
{
    Task<List<Supplier>> GetAllAsync( CancellationToken cancellationToken);
    Task<Supplier?> GetByIdAsync (Guid id, CancellationToken cancellationToken);
    Task AddAsync (Supplier  supplier, CancellationToken cancellationToken);
    Task UpdateAsync(Supplier supplier, CancellationToken cancellationToken);
    Task<Supplier?> GetByNameAsync(string name, CancellationToken cancellationToken);
}