using Microsoft.EntityFrameworkCore;
using Warehouse.Domain.Entities;
using Warehouse.Domain.Interfaces;

namespace Warehouse.Infrastructure.Persistence;

public class ProductRepository : IProductRepository
{
    private readonly WarehouseDbContext _context;

    public ProductRepository(WarehouseDbContext context)
    {
        _context = context;
    }

    public List<Product> GetAll()
    {
        return _context.Products
            .Include(product => product.Supplier)
            .ToList();
    }

    public Product? GetById(Guid id)
    {
        return _context.Products
            .Include(product => product.Supplier)
            .FirstOrDefault(product => product.Id == id);
    }

    public void Add(Product product)
    {
        _context.Products.Add(product);
        _context.SaveChanges();
    }

    public void Update(Product product)
    {
        _context.Products.Update(product);
        _context.SaveChanges();
    }

    public List<Product> Search(string? name, string? supplier)
    {
        var query = _context.Products
            .Include(product => product.Supplier)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(name))
        {
            query = query.Where(product =>
                product.Name.Contains(name));
        }

        if (!string.IsNullOrWhiteSpace(supplier))
        {
            query = query.Where(product =>
                product.Supplier != null &&
                product.Supplier.Name.Contains(supplier));
        }

        return query.ToList();
    }
}