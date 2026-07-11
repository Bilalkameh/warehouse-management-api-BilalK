using MediatR;
using Warehouse.Domain.Interfaces;

namespace Warehouse.Application.Queries.Products.GetProductsBySupplier;

public class GetProductsBySupplierHandler
    : IRequestHandler<GetProductsBySupplierRequest, List<GetProductsBySupplierResponse>>
{
    private readonly IProductRepository _repository;

    public GetProductsBySupplierHandler(IProductRepository repository)
    {
        _repository = repository;
    }

    public Task<List<GetProductsBySupplierResponse>> Handle(
        GetProductsBySupplierRequest request,
        CancellationToken cancellationToken)
    {
        var products = _repository.GetBySupplier(
            request.SupplierName,
            request.Ascending);
        var response = new List<GetProductsBySupplierResponse>();

        foreach (var product in products)
        {
            response.Add(new GetProductsBySupplierResponse
            {
                Id = product.Id,
                Name = product.Name,
                SKU = product.Sku,
                Description = product.Description,
                Price = product.Price,
                QuantityInStock = product.QuantityInStock,
                SupplierName = product.Supplier.Name,
                ExpiryDate = product.ExpiryDate,
                CreatedAt = product.CreatedAt
            });
        }
        return Task.FromResult(response);
    }
}