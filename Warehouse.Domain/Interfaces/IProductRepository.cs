using Warehouse.Domain.Entities;

namespace Warehouse.Domain.Interfaces;

public interface IProductRepository
{
    List <Product> GetAll();
    Product? GetById(Guid id);
    void Add(Product  product);
    void Update(Product  product);
	List<Product> Search(string? name, string? supplier);
	
	// additional session 4 LINQ queries
	List<Product> GetBySupplier(string supplierName, bool ascending);

	List<(int ExpiryYear, int Count)> GroupByExpiryYear();

	List<(int ExpiryYear, string Country, int Count)> GroupByExpiryYearAndCountry();

	int GetTotalCount();

	List<Product> GetPaged(int pageSize, int pageNumber);

}