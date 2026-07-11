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

    public List<Supplier> GetAll()
    {
        return _context.Suppliers.ToList();
    }

    public Supplier? GetById(Guid id)
    {
        return _context.Suppliers
            .FirstOrDefault(supplier => supplier.SupplierId == id);
    }

    public void Add(Supplier supplier)
    {
        _context.Suppliers.Add(supplier);
        _context.SaveChanges();
    }

    public void Update(Supplier supplier)
    {
        _context.Suppliers.Update(supplier);
        _context.SaveChanges();
    }
}