using MediatR;
using Warehouse.Domain.Interfaces;

namespace Warehouse.Application.Queries.Products.GetProducts;

public class GetProductsHandler : IRequestHandler<GetProductsRequest, List<GetProductsResponse>>
{
    private readonly IProductRepository _repository;
    public GetProductsHandler(IProductRepository repository)
    {
        _repository = repository;
    }

    public Task<List<GetProductsResponse>> Handle(GetProductsRequest request, CancellationToken cancellationToken)
    {
        var products = _repository.GetAll();
        if (request.OnlyAvailable)
        {
            products = products.Where(product => product.QuantityInStock > 0).ToList();
        }

        var response = new List<GetProductsResponse>();

        foreach (var product in products)
        {
            response.Add(new GetProductsResponse
            {
                Id = product.Id,
                Name = product.Name,
                SKU = product.SKU,
                Description = product.Description,
                Price = product.Price,
                QuantityInStock = product.QuantityInStock,
                SupplierName = product.Supplier?.Name ?? string.Empty,
                ExpiryDate = product.ExpiryDate,
                IsArchived = product.IsArchived,
                CreatedAt = product.CreatedAt,
                LastUpdatedAt = product.LastUpdatedAt
            });
        }

        return Task.FromResult(response);
    }

}