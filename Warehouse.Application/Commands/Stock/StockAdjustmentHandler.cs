using AutoMapper;
using FluentValidation;
using MediatR;
using Warehouse.Application.Exceptions;
using Warehouse.Application.ViewModels;
using Warehouse.Domain.Entities;
using Warehouse.Domain.Interfaces;
using Microsoft.Extensions.Logging;
using Warehouse.Application.Cache;
using Warehouse.Application.Interfaces;
using Warehouse.Application.Services;

namespace Warehouse.Application.Commands.Stock;

public class StockAdjustmentHandler : IRequestHandler<StockAdjustmentRequest, ProductViewModel>
{
    private readonly IProductRepository _productRepository;
    private readonly IValidator<StockAdjustmentRequest> _validator;
    private readonly IMapper _mapper;
    private readonly ILogger<StockAdjustmentHandler> _logger;
    private readonly ICacheService _cache;
    private readonly LowStockEventService _lowStockEventService;

    public StockAdjustmentHandler(IProductRepository productRepository, IValidator<StockAdjustmentRequest> validator, 
        IMapper mapper, ILogger<StockAdjustmentHandler> logger,  ICacheService cache, LowStockEventService lowStockEventService)
    {
        _productRepository = productRepository;
        _validator = validator;
        _mapper = mapper;
        _logger = logger;
        _cache = cache;
        _lowStockEventService = lowStockEventService;
    }

    public async Task<ProductViewModel> Handle(StockAdjustmentRequest request, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        var product = await _productRepository.GetByIdAsync(request.ProductId, cancellationToken);

        if (product == null)
            throw new NotFoundException("Product was not found.");
        
        var previousQuantity = product.QuantityInStock;
        var newQuantity = product.QuantityInStock + request.QuantityChange;
        
        product.UpdateQuantity(newQuantity);

        var movement = new StockMovement(product, request.QuantityChange, request.Reason);

        await _productRepository.AdjustStockAsync(product, movement, cancellationToken);
        
        await _lowStockEventService.PublishIfStockBecameLowAsync(product, previousQuantity, cancellationToken);
        
        await _cache.RemoveAsync(ProductCacheKeys.ById(product.Id), cancellationToken);

        await _cache.RemoveAsync(ProductCacheKeys.All, cancellationToken);

        await _cache.RemoveAsync(ProductCacheKeys.Available, cancellationToken);
        
        _logger.LogInformation("Stock adjusted for product {ProductId} by {QuantityChange}. New quantity is {NewQuantity}",
            product.Id,
            request.QuantityChange,
            product.QuantityInStock);
        
            
        return _mapper.Map<ProductViewModel>(product);
    }
}