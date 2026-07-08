using Warehouse.Application.Queries.Products;
using Warehouse.Domain.Entities;
using Warehouse.Domain.Interfaces;

namespace Warehouse.Application.Handlers.Products;

public class GetProductByIdHandler
{ 
    private readonly IProductRepository _repository;

    public GetProductByIdHandler(IProductRepository repository)
    {
        _repository = repository;
    }
    
    public Product? Handle(GetProductByIdQuery query)
    {
        return _repository.GetById(query.ProductId);
    }
}