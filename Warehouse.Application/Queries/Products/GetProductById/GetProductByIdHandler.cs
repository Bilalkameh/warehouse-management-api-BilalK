using AutoMapper;
using MediatR;
using Warehouse.Application.ViewModels;
using Warehouse.Application.Exceptions;
using Warehouse.Domain.Interfaces;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Warehouse.Application.Cache;
using Warehouse.Application.Interfaces;

namespace Warehouse.Application.Queries.Products.GetProductById;

public class GetProductByIdHandler
    : IRequestHandler<GetProductByIdRequest, ProductViewModel?>
{
    private readonly IProductRepository _repository;
    private readonly IMapper _mapper;
    private readonly ILogger<GetProductByIdHandler> _logger;
    private readonly ICacheService _cache;

    public GetProductByIdHandler(IProductRepository repository, IMapper mapper,  ILogger<GetProductByIdHandler> logger, ICacheService cache)
    {
        _repository = repository;
        _mapper = mapper;
        _logger = logger;
        _cache = cache;
    }

    public async Task<ProductViewModel?> Handle(GetProductByIdRequest request, CancellationToken cancellationToken)
    {
        
        var cacheKey = ProductCacheKeys.ById(request.ProductId);
        var cachedProduct = await _cache.GetAsync(cacheKey, cancellationToken);
        
        if (cachedProduct != null)
        {
            _logger.LogInformation("Product {ProductId} loaded from cache", request.ProductId);

            return JsonSerializer.Deserialize<ProductViewModel>(cachedProduct)!;
        }
        
        var product = await _repository.GetByIdAsync(request.ProductId, cancellationToken);

        if (product == null)
            throw new NotFoundException("Product was not found.");
        
        var productViewModel = _mapper.Map<ProductViewModel>(product);
        
        await _cache.SetAsync(cacheKey,JsonSerializer.Serialize(productViewModel),TimeSpan.FromMinutes(5), cancellationToken);
        
        _logger.LogInformation("Product {ProductId} added to cache", request.ProductId);
        return productViewModel;
    }
}