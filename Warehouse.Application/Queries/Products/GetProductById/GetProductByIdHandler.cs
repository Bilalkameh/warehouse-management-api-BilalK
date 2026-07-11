using MediatR;
using Warehouse.Domain.Interfaces;

namespace Warehouse.Application.Queries.Products.GetProductById;

public class GetProductByIdHandler : IRequestHandler<GetProductByIdRequest, GetProductByIdResponse?>
{
    private readonly IProductRepository _repository;

    public GetProductByIdHandler(IProductRepository repository)
    {
        _repository = repository;
    }

    public Task<GetProductByIdResponse?> Handle(GetProductByIdRequest request, CancellationToken cancellationToken)
    {
        var product = _repository.GetById(request.ProductId);

        if (product == null)
        {
            return Task.FromResult<GetProductByIdResponse?>(null);
        }

        var response = new GetProductByIdResponse
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
        };

        return Task.FromResult<GetProductByIdResponse?>(response);
    }

}