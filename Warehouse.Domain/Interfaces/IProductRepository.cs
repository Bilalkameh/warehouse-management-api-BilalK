using Warehouse.Domain.Entities;

namespace Warehouse.Domain.Interfaces;

public interface IProductRepository
{
    List <Product> GetAll();
    Product? GetById(string id);
    void Add(Product  product);
    void Update(Product  product);
}