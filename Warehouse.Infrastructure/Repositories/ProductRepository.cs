using Microsoft.EntityFrameworkCore;
using Warehouse.Domain.Entities;
using Warehouse.Domain.Interfaces;
using Warehouse.Infrastructure.Models;

namespace Warehouse.Infrastructure.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly WarehouseDbFirstContext _context;

    public ProductRepository(WarehouseDbFirstContext context)
    {
        _context = context;
    }

    public List<Product> GetAll()
    {
        return _context.Products
            .Include(product => product.Supplier)
            .Where(product => !product.IsArchived)
            .ToList();
    }

    public Product? GetById(Guid id)
    {
        return _context.Products
            .Include(product => product.Supplier)
            .Include(product => product.ProductImages)
            .FirstOrDefault(product =>
                product.Id == id &&
                !product.IsArchived);
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
            .Where(product => !product.IsArchived);

        if (!string.IsNullOrWhiteSpace(name))
        {
            query = query.Where(product =>
                product.Name.ToLower().Contains(name.ToLower()));
        }

        if (!string.IsNullOrWhiteSpace(supplier))
        {
            query = query.Where(product =>
                product.Supplier.Name.ToLower().Contains(supplier.ToLower()));
        }

        return query.ToList();
    }
}