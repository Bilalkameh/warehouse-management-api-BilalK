using System.Text.Json;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Warehouse.Application.Cache;
using Warehouse.Application.Exceptions;
using Warehouse.Application.Interfaces;
using Warehouse.Application.ViewModels;
using Warehouse.Domain.Interfaces;

namespace Warehouse.Application.Queries.Products.GetProductById;

public class GetProductByIdHandler : IRequestHandler<GetProductByIdRequest, ProductViewModel>
{
    private static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(5);
    private readonly IProductRepository _productRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<GetProductByIdHandler> _logger;
    private readonly ICacheService _cache;

    public GetProductByIdHandler(IProductRepository productRepository, IMapper mapper, ILogger<GetProductByIdHandler> logger, ICacheService cache)
    {
        _productRepository = productRepository;
        _mapper = mapper;
        _logger = logger;
        _cache = cache;
    }

    public async Task<ProductViewModel> Handle(GetProductByIdRequest request, CancellationToken cancellationToken)
    {
        var cacheKey = ProductCacheKeys.ById(request.ProductId);

        var cachedProduct = await _cache.GetAsync(cacheKey, cancellationToken);

        if (cachedProduct is not null)
        {
            var productViewModel = JsonSerializer.Deserialize<ProductViewModel>(cachedProduct);

            if (productViewModel is not null)
            {
                _logger.LogInformation("Product {ProductId} loaded from cache", request.ProductId);

                return productViewModel;
            }
        }

        var product = await _productRepository.GetByIdAsync(request.ProductId, cancellationToken)
            ?? throw new NotFoundException("Product was not found.");

        var result = _mapper.Map<ProductViewModel>(product);

        await _cache.SetAsync(cacheKey, JsonSerializer.Serialize(result), CacheDuration, cancellationToken);

        _logger.LogInformation("Product {ProductId} added to cache", request.ProductId);

        return result;
    }
}