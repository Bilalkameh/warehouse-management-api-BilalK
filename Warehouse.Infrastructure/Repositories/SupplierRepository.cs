using Warehouse.Domain.Entities;
using Warehouse.Domain.Interfaces;
using Warehouse.Infrastructure.Models;

namespace Warehouse.Infrastructure.Repositories;

public class SupplierRepository : ISupplierRepository
{
    private readonly WarehouseDbFirstContext _context;

    public SupplierRepository(WarehouseDbFirstContext context)
    {
        _context = context;
    }

    public List<Supplier> GetAll()
    {
        return _context.Suppliers
            .Where(supplier => supplier.IsActive)
            .ToList();
    }

    public Supplier? GetById(Guid id)
    {
        return _context.Suppliers
            .FirstOrDefault(supplier =>
                supplier.Id == id &&
                supplier.IsActive);
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