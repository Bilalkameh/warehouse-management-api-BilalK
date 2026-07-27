using MediatR;
using Warehouse.Application.Cache;
using Warehouse.Domain.Interfaces;
using Warehouse.Application.Exceptions;
using Warehouse.Application.Interfaces;
using Warehouse.Application.Services;

namespace Warehouse.Application.Commands.Products.UpdateProductQuantity;

public class UpdateProductQuantityHandler : IRequestHandler<UpdateProductQuantityRequest, UpdateProductQuantityResponse>
{
    private readonly IProductRepository _repository;
    private readonly ICacheService _cache;
    private readonly LowStockEventService _lowStockEventService;

    public UpdateProductQuantityHandler(IProductRepository repository, ICacheService cache, LowStockEventService lowStockEventService)
    {
        _repository = repository;
        _cache = cache;
        _lowStockEventService = lowStockEventService;
        
    }

    public async Task<UpdateProductQuantityResponse> Handle(UpdateProductQuantityRequest request, CancellationToken cancellationToken)
    {
        var product = await _repository.GetByIdAsync(request.ProductId, cancellationToken);

        if (product == null)
            throw new NotFoundException("Product was not found.");
        
        
        var previousQuantity = product.QuantityInStock;

        product.UpdateQuantity(request.Quantity);

        await _repository.UpdateAsync(product, cancellationToken);
        
        await _lowStockEventService.PublishIfStockBecameLowAsync(product, previousQuantity, cancellationToken);
        
        await _cache.RemoveAsync(ProductCacheKeys.ById(product.Id), cancellationToken);
        await _cache.RemoveAsync(ProductCacheKeys.All, cancellationToken);
        await _cache.RemoveAsync(ProductCacheKeys.Available, cancellationToken);

        return new UpdateProductQuantityResponse
        {
            Success = true
        };
    }
}