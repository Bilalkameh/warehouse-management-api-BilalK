using MediatR;
using Warehouse.Domain.Interfaces;

namespace Warehouse.Application.Queries.Products.SearchProducts;

public class SearchProductsHandler : IRequestHandler<SearchProductsRequest, List<SearchProductsResponse>>
{
    private readonly IProductRepository _repository;

    public SearchProductsHandler(IProductRepository repository)
    {
        _repository = repository;
    }

    public Task<List<SearchProductsResponse>> Handle(
        SearchProductsRequest request,
        CancellationToken cancellationToken)
    {
        var products = _repository.Search(
            request.Name,
            request.SupplierName);
        var response = new List<SearchProductsResponse>();

        foreach (var product in products)
        {
            response.Add(new SearchProductsResponse
            {
                Id = product.Id,
                Name = product.Name,
                SKU = product.Sku,
                Description = product.Description,
                Price = product.Price,
                QuantityInStock = product.QuantityInStock,
                SupplierName = product.Supplier.Name,
                ExpiryDate = product.ExpiryDate,
                IsArchived = product.IsArchived,
                CreatedAt = product.CreatedAt,
                LastUpdatedAt = product.LastUpdatedAt
            });
        }
        return Task.FromResult(response);
    }
}