using AutoMapper;
using MediatR;
using Warehouse.Application.ViewModels;
using Warehouse.Domain.Interfaces;

namespace Warehouse.Application.Queries.Products.GetOutOfStockProducts;

public class GetOutOfStockProductsHandler : IRequestHandler<GetOutOfStockProductsRequest, List<ProductViewModel>>
{
    private readonly IProductRepository _productRepository;
    private readonly IMapper _mapper;

    public GetOutOfStockProductsHandler(IProductRepository productRepository, IMapper mapper)
    {
        _productRepository = productRepository;
        _mapper = mapper;
    }

    public async Task<List<ProductViewModel>> Handle(GetOutOfStockProductsRequest request, CancellationToken cancellationToken)
    {
        var products = await _productRepository.GetOutOfStockAsync(cancellationToken);

        return _mapper.Map<List<ProductViewModel>>(products);
    }
}