using MediatR;
using Warehouse.Domain.Entities;
using Warehouse.Domain.Interfaces;

namespace Warehouse.Application.Commands.Products.CreateProduct;

public class CreateProductHandler : IRequestHandler<CreateProductRequest, CreateProductResponse>
{
    private readonly IProductRepository _productRepository;
    private readonly ISupplierRepository _supplierRepository;

    public CreateProductHandler(
        IProductRepository productRepository,
        ISupplierRepository supplierRepository)
    {
        _productRepository = productRepository;
        _supplierRepository = supplierRepository;
    }

    public Task<CreateProductResponse> Handle(
        CreateProductRequest request,
        CancellationToken cancellationToken)
    {
        var supplier = _supplierRepository
            .GetAll()
            .FirstOrDefault(supplier =>
                supplier.Name.Equals(
                    request.SupplierName,
                    StringComparison.OrdinalIgnoreCase));

        if (supplier == null)
        {
            throw new Exception("Supplier not found.");
        }

        var product = new Product
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Sku = request.SKU,
            Description = request.Description,
            Price = request.Price,
            QuantityInStock = request.QuantityInStock,
            SupplierId = supplier.Id,
            Supplier = supplier,
            ExpiryDate = request.ExpiryDate,
            IsArchived = false,
            CreatedAt = DateTime.UtcNow,
            LastUpdatedAt = DateTime.UtcNow
        };

        _productRepository.Add(product);

        var response = new CreateProductResponse
        {
            Id = product.Id,
            Name = product.Name,
            SKU = product.Sku,
            Description = product.Description,
            Price = product.Price,
            QuantityInStock = product.QuantityInStock,
            SupplierName = supplier.Name,
            ExpiryDate = product.ExpiryDate,
            IsArchived = product.IsArchived,
            CreatedAt = product.CreatedAt,
            LastUpdatedAt = product.LastUpdatedAt
        };

        return Task.FromResult(response);
    }
}