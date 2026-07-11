using AutoMapper;
using MediatR;
using Warehouse.Application.ViewModels;
using Warehouse.Domain.Interfaces;

namespace Warehouse.Application.Queries.Products.SearchProducts;

public class SearchProductsHandler
    : IRequestHandler<SearchProductsRequest, List<ProductViewModel>>
{
    private readonly IProductRepository _repository;
    private readonly IMapper _mapper;

    public SearchProductsHandler(
        IProductRepository repository,
        IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public Task<List<ProductViewModel>> Handle(
        SearchProductsRequest request,
        CancellationToken cancellationToken)
    {
        var products = _repository.Search(
            request.Name,
            request.SupplierName);
        
        var viewModels =
            _mapper.Map<List<ProductViewModel>>(products);

        return Task.FromResult(viewModels);
    }
}