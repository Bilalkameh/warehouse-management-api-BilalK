using Warehouse.Application.Queries.Products;
using Warehouse.Domain.Entities;
using Warehouse.Domain.Interfaces;

namespace Warehouse.Application.Handlers.Products;

public class SearchProductsHandler
{
    private readonly IProductRepository _repository;

    public SearchProductsHandler(IProductRepository repository)
    {
        _repository = repository;
    }

    public List<Product> Handle(SearchProductsQuery query)
    {
        return _repository.Search(
            query.Name,
            query.SupplierName
        );
    }
}