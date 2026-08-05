using MediatR;
using Warehouse.Application.Cache;
using Warehouse.Application.Exceptions;
using Warehouse.Application.Interfaces;
using Warehouse.Application.Services;
using Warehouse.Domain.Interfaces;

namespace Warehouse.Application.Commands.Products.UpdateProductQuantity;

public class UpdateProductQuantityHandler : IRequestHandler<UpdateProductQuantityRequest, UpdateProductQuantityResponse>
{
    private readonly IProductRepository _productRepository;
    private readonly ICacheService _cache;
    private readonly LowStockEventService _lowStockEventService;

    public UpdateProductQuantityHandler(IProductRepository productRepository, ICacheService cache, LowStockEventService lowStockEventService)
    {
        _productRepository = productRepository;
        _cache = cache;
        _lowStockEventService = lowStockEventService;
    }

    public async Task<UpdateProductQuantityResponse> Handle(UpdateProductQuantityRequest request, CancellationToken cancellationToken)
    {
        var product = await _productRepository.GetByIdAsync(request.ProductId, cancellationToken)
                      ?? throw new NotFoundException("Product was not found.");

        var previousQuantity = product.QuantityInStock;

        product.UpdateQuantity(request.Quantity);

        await _productRepository.UpdateAsync(product, cancellationToken);

        await _lowStockEventService.PublishIfStockBecameLowAsync(product, previousQuantity, cancellationToken);

        await ProductCacheInvalidator.InvalidateProductAsync(_cache, product.Id, cancellationToken);

        return new UpdateProductQuantityResponse
        {
            Success = true
        };
    }
}