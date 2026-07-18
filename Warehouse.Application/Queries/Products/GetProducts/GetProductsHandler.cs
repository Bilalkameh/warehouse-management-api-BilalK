using AutoMapper;
using MediatR;
using Warehouse.Application.ViewModels;
using Warehouse.Domain.Interfaces;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Warehouse.Application.Cache;
using Warehouse.Application.Interfaces;

namespace Warehouse.Application.Queries.Products.GetProducts;

public class GetProductsHandler
    : IRequestHandler<GetProductsRequest, List<ProductViewModel>>
{
    private readonly IProductRepository _repository;
    private readonly IMapper _mapper;
    private readonly ILogger<GetProductsHandler> _logger;
    private readonly ICacheService _cache;

    public GetProductsHandler(IProductRepository repository, IMapper mapper,  ILogger<GetProductsHandler> logger, ICacheService cache)
    {
        _repository = repository;
        _mapper = mapper;
        _logger = logger;
        _cache = cache;
    }

    public async Task<List<ProductViewModel>> Handle(GetProductsRequest request, CancellationToken cancellationToken)
    {
        
        var cacheKey = request.OnlyAvailable
            ? ProductCacheKeys.Available
            : ProductCacheKeys.All;
        
        var cachedProducts = await _cache.GetAsync(ProductCacheKeys.All, cancellationToken);
        
        if (cachedProducts != null)
        {
            _logger.LogInformation("All products loaded from cache using key {CacheKey}", cacheKey);

            return JsonSerializer.Deserialize<List<ProductViewModel>>(cachedProducts)!;
        }
        
        var products = request.OnlyAvailable
            ? await _repository.GetAvailableAsync(cancellationToken)
            : await _repository.GetAllAsync(cancellationToken);
        
        var productViewModels = _mapper.Map<List<ProductViewModel>>(products);
        
        await _cache.SetAsync(cacheKey, JsonSerializer.Serialize(productViewModels),TimeSpan.FromMinutes(5), cancellationToken);
        
        _logger.LogInformation("Products added to cache using key {CacheKey}", cacheKey);
        
        return productViewModels;
    }
}