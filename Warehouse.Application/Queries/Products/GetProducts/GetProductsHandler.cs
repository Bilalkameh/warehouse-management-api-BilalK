using AutoMapper;
using MediatR;
using Warehouse.Application.ViewModels;
using Warehouse.Domain.Interfaces;

namespace Warehouse.Application.Queries.Products.GetProducts;

public class GetProductsHandler
    : IRequestHandler<GetProductsRequest, List<ProductViewModel>>
{
    private readonly IProductRepository _repository;
    private readonly IMapper _mapper;

    public GetProductsHandler(
        IProductRepository repository,
        IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<List<ProductViewModel>> Handle(GetProductsRequest request, CancellationToken cancellationToken)
    {
        var products = request.OnlyAvailable
            ? await _repository.GetAvailableAsync(cancellationToken)
            : await _repository.GetAllAsync(cancellationToken);

        return _mapper.Map<List<ProductViewModel>>(products);
    }
}