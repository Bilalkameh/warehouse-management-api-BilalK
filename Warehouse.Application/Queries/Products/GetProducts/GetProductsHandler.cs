using System.Text.Json;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Warehouse.Application.Cache;
using Warehouse.Application.Interfaces;
using Warehouse.Application.ViewModels;
using Warehouse.Domain.Interfaces;

namespace Warehouse.Application.Queries.Products.GetProducts;

public class GetProductsHandler : IRequestHandler<GetProductsRequest, List<ProductViewModel>>
{
    private static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(5);

    private readonly IProductRepository _productRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<GetProductsHandler> _logger;
    private readonly ICacheService _cache;

    public GetProductsHandler(IProductRepository productRepository, IMapper mapper, ILogger<GetProductsHandler> logger, ICacheService cache)
    {
        _productRepository = productRepository;
        _mapper = mapper;
        _logger = logger;
        _cache = cache;
    }

    public async Task<List<ProductViewModel>> Handle(GetProductsRequest request, CancellationToken cancellationToken)
    {
        var cacheKey = request.OnlyAvailable
            ? ProductCacheKeys.Available
            : ProductCacheKeys.All;

        var cachedProducts = await _cache.GetAsync(cacheKey, cancellationToken);

        if (cachedProducts is not null)
        {
            var productViewModels = JsonSerializer.Deserialize<List<ProductViewModel>>(cachedProducts);

            if (productViewModels is not null)
            {
                _logger.LogInformation("Products loaded from cache using key {CacheKey}", cacheKey);

                return productViewModels;
            }
        }

        var products = request.OnlyAvailable
            ? await _productRepository.GetAvailableAsync(cancellationToken)
            : await _productRepository.GetAllAsync(cancellationToken);

        var result = _mapper.Map<List<ProductViewModel>>(products);

        await _cache.SetAsync(cacheKey, JsonSerializer.Serialize(result), CacheDuration, cancellationToken);

        _logger.LogInformation("Products added to cache using key {CacheKey}", cacheKey);

        return result;
    }
}