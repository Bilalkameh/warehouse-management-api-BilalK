Warehouse.Application/Cache/ProductCacheInvalidator.cs — new file
using Warehouse.Application.Interfaces;

namespace Warehouse.Application.Cache;

internal static class ProductCacheInvalidator
{
public static async Task InvalidateProductAsync(
ICacheService cache,
Guid productId,
CancellationToken cancellationToken)
{
await cache.RemoveAsync(
ProductCacheKeys.ById(productId),
cancellationToken);

        await InvalidateProductListsAsync(cache, cancellationToken);
    }

    public static async Task InvalidateProductListsAsync(
        ICacheService cache,
        CancellationToken cancellationToken)
    {
        await cache.RemoveAsync(
            ProductCacheKeys.All,
            cancellationToken);

        await cache.RemoveAsync(
            ProductCacheKeys.Available,
            cancellationToken);
    }
}
Warehouse.Application/Commands/Products/ArchiveProduct/ArchiveProductHandler.cs
using MediatR;
using Warehouse.Application.Cache;
using Warehouse.Application.Exceptions;
using Warehouse.Application.Interfaces;
using Warehouse.Domain.Interfaces;

namespace Warehouse.Application.Commands.Products.ArchiveProduct;

public class ArchiveProductHandler
: IRequestHandler<ArchiveProductRequest, ArchiveProductResponse>
{
private readonly IProductRepository _productRepository;
private readonly ICacheService _cache;

    public ArchiveProductHandler(
        IProductRepository productRepository,
        ICacheService cache)
    {
        _productRepository = productRepository;
        _cache = cache;
    }

    public async Task<ArchiveProductResponse> Handle(
        ArchiveProductRequest request,
        CancellationToken cancellationToken)
    {
        var product = await _productRepository.GetByIdAsync(
                request.ProductId,
                cancellationToken)
            ?? throw new NotFoundException("Product was not found.");

        product.Archive();

        await _productRepository.UpdateAsync(
            product,
            cancellationToken);

        await ProductCacheInvalidator.InvalidateProductAsync(
            _cache,
            product.Id,
            cancellationToken);

        return new ArchiveProductResponse
        {
            Success = true
        };
    }
}
Warehouse.Application/Commands/Products/CreateProduct/CreateProductHandler.cs
using AutoMapper;
using MediatR;
using Warehouse.Application.Cache;
using Warehouse.Application.Exceptions;
using Warehouse.Application.Interfaces;
using Warehouse.Application.ViewModels;
using Warehouse.Domain.Entities;
using Warehouse.Domain.Interfaces;

namespace Warehouse.Application.Commands.Products.CreateProduct;

public class CreateProductHandler
: IRequestHandler<CreateProductRequest, ProductViewModel>
{
private readonly IProductRepository _productRepository;
private readonly ISupplierRepository _supplierRepository;
private readonly IMapper _mapper;
private readonly ICacheService _cache;

    public CreateProductHandler(
        IProductRepository productRepository,
        ISupplierRepository supplierRepository,
        IMapper mapper,
        ICacheService cache)
    {
        _productRepository = productRepository;
        _supplierRepository = supplierRepository;
        _mapper = mapper;
        _cache = cache;
    }

    public async Task<ProductViewModel> Handle(
        CreateProductRequest request,
        CancellationToken cancellationToken)
    {
        var skuExists = await _productRepository.ExistsBySkuAsync(
            request.SKU,
            cancellationToken);

        if (skuExists)
            throw new ConflictException(
                "A product with this SKU already exists.");

        var supplier = await _supplierRepository.GetByNameAsync(
                request.SupplierName,
                cancellationToken)
            ?? throw new NotFoundException("Supplier was not found.");

        var product = new Product(
            request.Name,
            request.SKU,
            request.Description,
            request.Price,
            request.QuantityInStock,
            supplier,
            request.ExpiryDate);

        await _productRepository.AddAsync(
            product,
            cancellationToken);

        await ProductCacheInvalidator.InvalidateProductListsAsync(
            _cache,
            cancellationToken);

        return _mapper.Map<ProductViewModel>(product);
    }
}
Warehouse.Application/Commands/Products/UpdateProductPrice/UpdateProductPriceHandler.cs
using MediatR;
using Warehouse.Application.Cache;
using Warehouse.Application.Exceptions;
using Warehouse.Application.Interfaces;
using Warehouse.Domain.Interfaces;

namespace Warehouse.Application.Commands.Products.UpdateProductPrice;

public class UpdateProductPriceHandler
: IRequestHandler<UpdateProductPriceRequest, UpdateProductPriceResponse>
{
private readonly IProductRepository _productRepository;
private readonly ICacheService _cache;

    public UpdateProductPriceHandler(
        IProductRepository productRepository,
        ICacheService cache)
    {
        _productRepository = productRepository;
        _cache = cache;
    }

    public async Task<UpdateProductPriceResponse> Handle(
        UpdateProductPriceRequest request,
        CancellationToken cancellationToken)
    {
        var product = await _productRepository.GetByIdAsync(
                request.ProductId,
                cancellationToken)
            ?? throw new NotFoundException("Product was not found.");

        product.UpdatePrice(request.Price);

        await _productRepository.UpdateAsync(
            product,
            cancellationToken);

        await ProductCacheInvalidator.InvalidateProductAsync(
            _cache,
            product.Id,
            cancellationToken);

        return new UpdateProductPriceResponse
        {
            Success = true
        };
    }
}
Warehouse.Application/Commands/Products/UpdateProductQuantity/UpdateProductQuantityHandler.cs
using MediatR;
using Warehouse.Application.Cache;
using Warehouse.Application.Exceptions;
using Warehouse.Application.Interfaces;
using Warehouse.Application.Services;
using Warehouse.Domain.Interfaces;

namespace Warehouse.Application.Commands.Products.UpdateProductQuantity;

public class UpdateProductQuantityHandler
: IRequestHandler<UpdateProductQuantityRequest, UpdateProductQuantityResponse>
{
private readonly IProductRepository _productRepository;
private readonly ICacheService _cache;
private readonly LowStockEventService _lowStockEventService;

    public UpdateProductQuantityHandler(
        IProductRepository productRepository,
        ICacheService cache,
        LowStockEventService lowStockEventService)
    {
        _productRepository = productRepository;
        _cache = cache;
        _lowStockEventService = lowStockEventService;
    }

    public async Task<UpdateProductQuantityResponse> Handle(
        UpdateProductQuantityRequest request,
        CancellationToken cancellationToken)
    {
        var product = await _productRepository.GetByIdAsync(
                request.ProductId,
                cancellationToken)
            ?? throw new NotFoundException("Product was not found.");

        var previousQuantity = product.QuantityInStock;

        product.UpdateQuantity(request.Quantity);

        await _productRepository.UpdateAsync(
            product,
            cancellationToken);

        await _lowStockEventService.PublishIfStockBecameLowAsync(
            product,
            previousQuantity,
            cancellationToken);

        await ProductCacheInvalidator.InvalidateProductAsync(
            _cache,
            product.Id,
            cancellationToken);

        return new UpdateProductQuantityResponse
        {
            Success = true
        };
    }
}
Warehouse.Application/Queries/Products/GetProductById/GetProductByIdHandler.cs
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

public class GetProductByIdHandler
: IRequestHandler<GetProductByIdRequest, ProductViewModel>
{
private static readonly TimeSpan CacheDuration =
TimeSpan.FromMinutes(5);

    private readonly IProductRepository _productRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<GetProductByIdHandler> _logger;
    private readonly ICacheService _cache;

    public GetProductByIdHandler(
        IProductRepository productRepository,
        IMapper mapper,
        ILogger<GetProductByIdHandler> logger,
        ICacheService cache)
    {
        _productRepository = productRepository;
        _mapper = mapper;
        _logger = logger;
        _cache = cache;
    }

    public async Task<ProductViewModel> Handle(
        GetProductByIdRequest request,
        CancellationToken cancellationToken)
    {
        var cacheKey = ProductCacheKeys.ById(request.ProductId);

        var cachedProduct = await _cache.GetAsync(
            cacheKey,
            cancellationToken);

        if (cachedProduct is not null)
        {
            var productViewModel =
                JsonSerializer.Deserialize<ProductViewModel>(
                    cachedProduct);

            if (productViewModel is not null)
            {
                _logger.LogInformation(
                    "Product {ProductId} loaded from cache",
                    request.ProductId);

                return productViewModel;
            }
        }

        var product = await _productRepository.GetByIdAsync(
                request.ProductId,
                cancellationToken)
            ?? throw new NotFoundException("Product was not found.");

        var result = _mapper.Map<ProductViewModel>(product);

        await _cache.SetAsync(
            cacheKey,
            JsonSerializer.Serialize(result),
            CacheDuration,
            cancellationToken);

        _logger.LogInformation(
            "Product {ProductId} added to cache",
            request.ProductId);

        return result;
    }
}
Warehouse.Application/Queries/Products/GetProducts/GetProductsHandler.cs
using System.Text.Json;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Warehouse.Application.Cache;
using Warehouse.Application.Interfaces;
using Warehouse.Application.ViewModels;
using Warehouse.Domain.Interfaces;

namespace Warehouse.Application.Queries.Products.GetProducts;

public class GetProductsHandler
: IRequestHandler<GetProductsRequest, List<ProductViewModel>>
{
private static readonly TimeSpan CacheDuration =
TimeSpan.FromMinutes(5);

    private readonly IProductRepository _productRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<GetProductsHandler> _logger;
    private readonly ICacheService _cache;

    public GetProductsHandler(
        IProductRepository productRepository,
        IMapper mapper,
        ILogger<GetProductsHandler> logger,
        ICacheService cache)
    {
        _productRepository = productRepository;
        _mapper = mapper;
        _logger = logger;
        _cache = cache;
    }

    public async Task<List<ProductViewModel>> Handle(
        GetProductsRequest request,
        CancellationToken cancellationToken)
    {
        var cacheKey = request.OnlyAvailable
            ? ProductCacheKeys.Available
            : ProductCacheKeys.All;

        var cachedProducts = await _cache.GetAsync(
            cacheKey,
            cancellationToken);

        if (cachedProducts is not null)
        {
            var productViewModels =
                JsonSerializer.Deserialize<List<ProductViewModel>>(
                    cachedProducts);

            if (productViewModels is not null)
            {
                _logger.LogInformation(
                    "Products loaded from cache using key {CacheKey}",
                    cacheKey);

                return productViewModels;
            }
        }

        var products = request.OnlyAvailable
            ? await _productRepository.GetAvailableAsync(
                cancellationToken)
            : await _productRepository.GetAllAsync(
                cancellationToken);

        var result = _mapper.Map<List<ProductViewModel>>(products);

        await _cache.SetAsync(
            cacheKey,
            JsonSerializer.Serialize(result),
            CacheDuration,
            cancellationToken);

        _logger.LogInformation(
            "Products added to cache using key {CacheKey}",
            cacheKey);

        return result;
    }
}
Warehouse.Infrastructure/Persistence/ProductRepository.cs
using Microsoft.EntityFrameworkCore;
using Warehouse.Domain.Entities;
using Warehouse.Domain.Interfaces;

namespace Warehouse.Infrastructure.Persistence;

public class ProductRepository : IProductRepository
{
private readonly WarehouseDbContext _context;

    public ProductRepository(WarehouseDbContext context)
    {
        _context = context;
    }

    public async Task<List<Product>> GetAllAsync(
        CancellationToken cancellationToken)
    {
        return await _context.Products
            .AsNoTracking()
            .Include(product => product.Supplier)
            .ToListAsync(cancellationToken);
    }

    public async Task<Product?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        return await _context.Products
            .Include(product => product.Supplier)
            .FirstOrDefaultAsync(
                product => product.Id == id,
                cancellationToken);
    }

    public async Task<List<Product>> GetAvailableAsync(
        CancellationToken cancellationToken)
    {
        return await _context.Products
            .AsNoTracking()
            .Include(product => product.Supplier)
            .Where(product =>
                product.QuantityInStock > 0 &&
                !product.IsArchived)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(
        Product product,
        CancellationToken cancellationToken)
    {
        await _context.Products.AddAsync(
            product,
            cancellationToken);

        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(
        Product product,
        CancellationToken cancellationToken)
    {
        _context.Products.Update(product);

        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<List<Product>> SearchAsync(
        string? name,
        string? supplierName,
        CancellationToken cancellationToken)
    {
        var query = _context.Products
            .AsNoTracking()
            .Include(product => product.Supplier)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(name))
        {
            query = query.Where(product =>
                product.Name.Contains(name));
        }

        if (!string.IsNullOrWhiteSpace(supplierName))
        {
            query = query.Where(product =>
                product.Supplier.Name.Contains(supplierName));
        }

        return await query.ToListAsync(cancellationToken);
    }

    public async Task AdjustStockAsync(
        Product product,
        StockMovement movement,
        CancellationToken cancellationToken)
    {
        _context.Products.Update(product);

        await _context.StockMovements.AddAsync(
            movement,
            cancellationToken);

        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<List<Product>> GetExpiringProductsAsync(
        DateTime date,
        CancellationToken cancellationToken)
    {
        return await _context.Products
            .Where(product => product.ExpiryDate <= date)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<Product>> GetExpiringSoonAsync(
        DateTime startDate,
        DateTime endDateExclusive,
        CancellationToken cancellationToken)
    {
        return await _context.Products
            .AsNoTracking()
            .Include(product => product.Supplier)
            .Where(product =>
                product.ExpiryDate >= startDate &&
                product.ExpiryDate < endDateExclusive)
            .OrderBy(product => product.ExpiryDate)
            .ThenBy(product => product.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> ExistsBySkuAsync(
        string sku,
        CancellationToken cancellationToken)
    {
        return await _context.Products.AnyAsync(
            product => product.SKU == sku,
            cancellationToken);
    }
}
Warehouse.Infrastructure/Persistence/ProductImageRepository.cs
using Microsoft.EntityFrameworkCore;
using Warehouse.Domain.Entities;
using Warehouse.Domain.Interfaces;

namespace Warehouse.Infrastructure.Persistence;

public class ProductImageRepository : IProductImageRepository
{
private readonly WarehouseDbContext _context;

    public ProductImageRepository(WarehouseDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(
        ProductImage image,
        CancellationToken cancellationToken)
    {
        await _context.ProductImages.AddAsync(
            image,
            cancellationToken);

        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<List<ProductImage>> GetByProductIdAsync(
        Guid productId,
        CancellationToken cancellationToken)
    {
        return await _context.ProductImages
            .AsNoTracking()
            .Where(image => image.ProductId == productId)
            .ToListAsync(cancellationToken);
    }

    public async Task<ProductImage?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        return await _context.ProductImages
            .FirstOrDefaultAsync(
                image => image.Id == id,
                cancellationToken);
    }

    public async Task DeleteAsync(
        ProductImage image,
        CancellationToken cancellationToken)
    {
        _context.ProductImages.Remove(image);

        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task ReplaceAsync(
        ProductImage oldImage,
        ProductImage newImage,
        CancellationToken cancellationToken)
    {
        _context.ProductImages.Remove(oldImage);

        await _context.ProductImages.AddAsync(
            newImage,
            cancellationToken);

        await _context.SaveChangesAsync(cancellationToken);
    }
}