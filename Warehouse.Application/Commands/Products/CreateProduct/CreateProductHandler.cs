using MediatR;
using Warehouse.Domain.Entities;
using Warehouse.Domain.Interfaces;

namespace Warehouse.Application.Commands.Products.CreateProduct;

public class CreateProductHandler : IRequestHandler<CreateProductRequest, CreateProductResponse>
{
    private readonly IProductRepository _repository;

    public CreateProductHandler(IProductRepository repository)
    {
        _repository = repository;
    }

    public Task<CreateProductResponse> Handle(CreateProductRequest request, CancellationToken cancellationToken)
    {
        var product = new Product(request.Name, request.SKU, request.Description, request.Price,
            request.QuantityInStock, request.SupplierName, request.ExpiryDate);
        
        _repository.Add(product);
        var response = new CreateProductResponse
        {
            Id = product.Id,
            Name = product.Name,
            SKU = product.SKU,
            Description = product.Description,
            Price = product.Price,
            QuantityInStock = product.QuantityInStock,
            SupplierName = product.SupplierName,
            ExpiryDate = product.ExpiryDate,
            IsArchived = product.IsArchived,
            CreatedAt = product.CreatedAt,
            LastUpdatedAt = product.LastUpdatedAt
        };

        return Task.FromResult(response);
    }
}