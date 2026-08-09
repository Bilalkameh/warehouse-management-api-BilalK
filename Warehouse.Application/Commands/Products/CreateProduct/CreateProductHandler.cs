using AutoMapper;
using MediatR;
using Warehouse.Application.Cache;
using Warehouse.Application.Exceptions;
using Warehouse.Application.Interfaces;
using Warehouse.Application.ViewModels;
using Warehouse.Domain.Entities;
using Warehouse.Domain.Interfaces;

namespace Warehouse.Application.Commands.Products.CreateProduct;

public class CreateProductHandler : IRequestHandler<CreateProductRequest, ProductViewModel>
{
    private readonly IProductRepository _productRepository;
    private readonly ISupplierRepository _supplierRepository;
    private readonly IMapper _mapper;
    private readonly ICacheService _cache;

    public CreateProductHandler(IProductRepository productRepository, ISupplierRepository supplierRepository, IMapper mapper, ICacheService cache)
    {
        _productRepository = productRepository;
        _supplierRepository = supplierRepository;
        _mapper = mapper;
        _cache = cache;
    }

    public async Task<ProductViewModel> Handle(CreateProductRequest request, CancellationToken cancellationToken)
    {
        var skuExists = await _productRepository.ExistsBySkuAsync(request.SKU, cancellationToken);

        if (skuExists)
            throw new ConflictException("A product with this SKU already exists.");

        var supplier = await _supplierRepository.GetByNameAsync(request.SupplierName, cancellationToken)
            ?? throw new NotFoundException("Supplier was not found.");

        var product = new Product(
            request.Name,
            request.SKU,
            request.Description,
            request.Price,
            request.QuantityInStock,
            supplier,
            request.ExpiryDate);

        await _productRepository.AddAsync(product, cancellationToken);

        await ProductCacheInvalidator.InvalidateProductListsAsync(_cache, cancellationToken);

        return _mapper.Map<ProductViewModel>(product);
    }
}