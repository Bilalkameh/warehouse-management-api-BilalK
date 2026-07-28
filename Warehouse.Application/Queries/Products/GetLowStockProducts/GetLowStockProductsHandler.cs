using AutoMapper;
using MediatR;
using Microsoft.Extensions.Options;
using Warehouse.Application.Settings;
using Warehouse.Application.ViewModels;
using Warehouse.Domain.Interfaces;

namespace Warehouse.Application.Queries.Products.GetLowStockProducts;

public class GetLowStockProductsHandler : IRequestHandler<GetLowStockProductsRequest, List<ProductViewModel>>
{
    private readonly IProductRepository _repository;
    private readonly IMapper _mapper;
    private readonly LowStockSettings _settings;

    public GetLowStockProductsHandler(IProductRepository repository, IMapper mapper, IOptions<LowStockSettings> options)
    {
        _repository = repository;
        _mapper = mapper;
        _settings = options.Value;
    }

    public async Task<List<ProductViewModel>> Handle(GetLowStockProductsRequest request, CancellationToken cancellationToken)
    {
        var products = await _repository.GetAllAsync(cancellationToken);

        var lowStockProducts = products
            .Where(product => !product.IsArchived && product.QuantityInStock < _settings.Threshold)
            .ToList();

        return _mapper.Map<List<ProductViewModel>>(lowStockProducts);
    }
}