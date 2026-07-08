using Warehouse.Application.Commands.Products;
using Warehouse.Domain.Entities;
using Warehouse.Domain.Interfaces;

namespace Warehouse.Application.Handlers.Products;

public class CreateProductHandler
{
    private readonly IProductRepository _repository;

    public CreateProductHandler(IProductRepository repository)
    {
        _repository = repository;
    }
    
    public Product Handle(CreateProductCommand command)
    {
        var product = new Product(
            command.Name,
            command.SKU ?? string.Empty,
            command.Description,
            command.Price,
            command.QuantityInStock,
            command.SupplierName,
            command.ExpiryDate
        );
        _repository.Add(product);

        return product;
    }
}