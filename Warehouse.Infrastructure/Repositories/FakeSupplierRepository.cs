using Warehouse.Domain.Entities;
using Warehouse.Domain.Interfaces;

namespace Warehouse.Infrastructure.Repositories;


public class FakeSupplierRepository : ISupplierRepository
{
    private readonly List<Supplier> _suppliers = new();


    public FakeSupplierRepository()
    {
        var sup1 = new Supplier(
            "sup1",
            "lebanon",
            "sup1@mail.com",
            "+961-81-123-456");


        var sup2 = new Supplier(
            "sup2",
            "lebanon",
            "sup2@mail.com",
            "+961-71-654-123");


        var sup3 = new Supplier(
            "sup3",
            "usa",
            "sup3@mail.com",
            "+1-123-456");


        var sup4 = new Supplier(
            "sup4",
            "germany",
            "sup4@mail.com",
            "+49-123-333");


        _suppliers.AddRange(new[]
        {
            sup1,
            sup2,
            sup3,
            sup4
        });
    }


    public List<Supplier> GetAll()
    {
        return _suppliers
            .Where(s => s.IsActive)
            .ToList();
    }


    public Supplier? GetById(Guid id)
    {
        return _suppliers.FirstOrDefault(s => s.Id == id);
    }


    public void Add(Supplier supplier)
    {
        _suppliers.Add(supplier);
    }


    public void Update(Supplier supplier)
    {
        var existing = GetById(supplier.Id);

        if (existing == null)
            return;

        var index = _suppliers.IndexOf(existing);

        _suppliers[index] = supplier;
    }
}