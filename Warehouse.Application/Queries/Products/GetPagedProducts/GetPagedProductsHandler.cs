using MediatR;
using Warehouse.Domain.Interfaces;

namespace Warehouse.Application.Queries.Products.GetPagedProducts;

public class GetPagedProductsHandler
    : IRequestHandler<GetPagedProductsRequest, List<GetPagedProductsResponse>>
{
    private readonly IProductRepository _repository;

    public GetPagedProductsHandler(IProductRepository repository)
    {
        _repository = repository;
    }

    public Task<List<GetPagedProductsResponse>> Handle(
        GetPagedProductsRequest request,
        CancellationToken cancellationToken)
    {
        var products = _repository.GetPaged(
            request.PageSize,
            request.PageNumber);
        var response = new List<GetPagedProductsResponse>();

        foreach (var product in products)
        {
            response.Add(new GetPagedProductsResponse
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