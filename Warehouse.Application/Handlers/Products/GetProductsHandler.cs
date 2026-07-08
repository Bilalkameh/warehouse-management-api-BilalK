using Warehouse.Application.Queries.Products;
using Warehouse.Domain.Entities;
using Warehouse.Domain.Interfaces;

namespace Warehouse.Application.Handlers.Products;


public class GetProductsHandler
{
    private readonly IProductRepository _repository;
    
    public GetProductsHandler(IProductRepository repository)
    {
        _repository = repository;
    }
    
    public List<Product> Handle(GetProductsQuery query)
    {
        return _repository.GetAll();
    }
}