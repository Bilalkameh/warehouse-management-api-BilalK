using Warehouse.Domain.Entities;
using Warehouse.Domain.Interfaces;

namespace Warehouse.Infrastructure.Repositories;

public class FakeProductRepository : IProductRepository
{
    private readonly List<Product> _products = new();


    public FakeProductRepository()
    {
        var laptop1 = new Product(
            "laptop1",
            "LAP-001",
            "powerful gaming laptop",
            1299.9,
            10,
            "laptopSupplier1",
            new DateTime(2029, 2, 7));

        var laptop2 = new Product(
            "laptop2",
            "LAP-002",
            "budget laptop",
            500,
            12,
            "laptopSupplier2",
            new DateTime(2028, 5, 30));

        var mouse1 = new Product(
            "mouse1",
            "MOU-001",
            "budget mouse",
            10,
            30,
            "mouseSupplier1",
            new DateTime(2028, 1, 1));

        var mouse2 = new Product(
            "mouse2",
            "MOU-002",
            "gaming mouse",
            40,
            15,
            "mouseSupplier2",
            new DateTime(2028, 1, 1));

        var keyboard1 = new Product(
            "keyboard1",
            "KEY-001",
            "gaming keyboard",
            60.99,
            20,
            "keyBoardSupplier1",
            new DateTime(2029, 1, 1));

        var keyboard2 = new Product(
            "keyboard2",
            "KEY-002",
            "budget keyboard",
            20,
            25,
            "keyBoardSupplier2",
            new DateTime(2029, 6, 30));

        var scanner1 = new Product(
            "scanner1",
            "SCN-001",
            "scanner 1",
            100,
            8,
            "scannerSupplier1",
            new DateTime(2029, 4, 1));

        var printer1 = new Product(
            "printer1",
            "PRI-001",
            "printer 1",
            120.99,
            7,
            "printerSupplier1",
            new DateTime(2028, 5, 1));

        var monitor1 = new Product(
            "monitor1",
            "MON-001",
            "OLED monitor",
            450.99,
            5,
            "monitorSupplier1",
            new DateTime(2029, 1, 1));

        var monitor2 = new Product(
            "monitor2",
            "MON-002",
            "budget monitor",
            100.99,
            17,
            "monitorSupplier2",
            new DateTime(2029, 1, 1));


        _products.AddRange(new[]
        {
            laptop1,
            laptop2,
            mouse1,
            mouse2,
            keyboard1,
            keyboard2,
            scanner1,
            printer1,
            monitor1,
            monitor2
        });
    }


    public List<Product> GetAll()
    {
        return _products
            .Where(p => !p.IsArchived)
            .ToList();
    }


    public Product? GetById(Guid id)
    {
        return _products.FirstOrDefault(p => p.Id == id);
    }


    public void Add(Product product)
    {
        _products.Add(product);
    }


    public void Update(Product product)
    {
        var existing = GetById(product.Id);

        if (existing == null)
            return;

        var index = _products.IndexOf(existing);
        _products[index] = product;
    }
    
    public List<Product> Search(string? name, string? supplier)
    {
        return _products
            .Where(p =>
                (string.IsNullOrWhiteSpace(name) ||
                 p.Name.Contains(name, StringComparison.OrdinalIgnoreCase))
                &&
                (string.IsNullOrWhiteSpace(supplier) ||
                 p.SupplierName.Contains(supplier, StringComparison.OrdinalIgnoreCase)))
            .ToList();
    }
}