using MediatR;
using Warehouse.Application.Cache;
using Warehouse.Domain.Interfaces;
using Warehouse.Application.Exceptions;
using Warehouse.Application.Interfaces;


namespace Warehouse.Application.Commands.Products.ArchiveProduct;

public class ArchiveProductHandler : IRequestHandler<ArchiveProductRequest, ArchiveProductResponse>
{
    private readonly IProductRepository _repository;
    private readonly ICacheService _cache;

    public ArchiveProductHandler(IProductRepository repository,  ICacheService cache)
    {
        _repository = repository;
        _cache = cache;
    }

    public async Task<ArchiveProductResponse> Handle(ArchiveProductRequest request, CancellationToken cancellationToken)
    {
        var product = await _repository.GetByIdAsync(request.ProductId, cancellationToken) 
                      ?? throw new NotFoundException("Product was not found."); 

        product.Archive();
        
        await _repository.UpdateAsync(product, cancellationToken);
        await ProductCacheInvalidator.InvalidateProductAsync(_cache, product.Id, cancellationToken);

        return new ArchiveProductResponse
        {
            Success = true
        };
    }
}