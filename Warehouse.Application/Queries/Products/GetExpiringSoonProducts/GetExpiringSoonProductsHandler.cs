using AutoMapper;
using MediatR;
using Warehouse.Application.ViewModels;
using Warehouse.Domain.Interfaces;

namespace Warehouse.Application.Queries.Products.GetExpiringSoonProducts;

public class GetExpiringSoonProductsHandler
    : IRequestHandler<GetExpiringSoonProductsRequest, List<ProductViewModel>>
{
    private const int ExpiringSoonDays = 30;

    private readonly IProductRepository _productRepository;
    private readonly IMapper _mapper;
    private readonly TimeProvider _timeProvider;

    public GetExpiringSoonProductsHandler(
        IProductRepository productRepository,
        IMapper mapper,
        TimeProvider timeProvider)
    {
        _productRepository = productRepository;
        _mapper = mapper;
        _timeProvider = timeProvider;
    }

    public async Task<List<ProductViewModel>> Handle(
        GetExpiringSoonProductsRequest request,
        CancellationToken cancellationToken)
    {
        var startDate = _timeProvider.GetUtcNow().UtcDateTime.Date;
        var endDateExclusive = startDate.AddDays(ExpiringSoonDays + 1);

        var products = await _productRepository.GetExpiringSoonAsync(
            startDate,
            endDateExclusive,
            cancellationToken);

        return _mapper.Map<List<ProductViewModel>>(products);
    }
}