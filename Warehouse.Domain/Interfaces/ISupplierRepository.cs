using Warehouse.Domain.Entities;

namespace Warehouse.Domain.Interfaces;


public interface ISupplierRepository
{
    List <Supplier> GetAll();
    Supplier? GetById(Guid id);
    void Add(Supplier  supplier);
    void Update(Supplier supplier);
}