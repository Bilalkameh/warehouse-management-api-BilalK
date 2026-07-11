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
    
    public List<Product> GetBySupplier(string supplierName, bool ascending)
    {
        var query = _context.Products
            .Include(product => product.Supplier)
            .Where(product =>
                !product.IsArchived && product.Supplier.Name.ToLower() == supplierName.ToLower());
        
        if (ascending)
        {
            query = query.OrderBy(product => product.CreatedAt);
        }
        else
        {
            query = query.OrderByDescending(product => product.CreatedAt);
        }
        return query.ToList();
    }
    
    public List<(int ExpiryYear, int Count)> GroupByExpiryYear()
    {
        return _context.Products
            .Where(product => !product.IsArchived)
            .GroupBy(product => product.ExpiryDate.Year)
            .Select(group => new
            {
                ExpiryYear = group.Key,
                Count = group.Count()
            })
            .AsEnumerable()
            .Select(group => (group.ExpiryYear, group.Count))
            .ToList();
    }

    public List<(int ExpiryYear, string Country, int Count)> GroupByExpiryYearAndCountry()
    {
        return _context.Products
            .Where(product => !product.IsArchived)
            .GroupBy(product => new
            {
                ExpiryYear = product.ExpiryDate.Year,
                Country = product.Supplier.Country
            })
            .Select(group => new
            {
                ExpiryYear = group.Key.ExpiryYear,
                Country = group.Key.Country,
                Count = group.Count()
            })
            .AsEnumerable()
            .Select(group =>
                (group.ExpiryYear, group.Country, group.Count))
            .ToList();
    }
    
    public int GetTotalCount()
    {
        return _context.Products
            .Count(product => !product.IsArchived);
    }

    public List<Product> GetPaged(int pageSize, int pageNumber)
    {
        return _context.Products
            .Include(product => product.Supplier)
            .Where(product => !product.IsArchived)
            .OrderBy(product => product.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToList();
    }
}


