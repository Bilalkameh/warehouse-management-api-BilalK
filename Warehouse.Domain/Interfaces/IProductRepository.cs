using Warehouse.Domain.Entities;

namespace Warehouse.Domain.Interfaces;

public interface IProductRepository
{
    List <Product> GetAll();
    Product? GetById(Guid id);
    void Add(Product  product);
    void Update(Product  product);
	List<Product> Search(string? name, string? supplier);
}