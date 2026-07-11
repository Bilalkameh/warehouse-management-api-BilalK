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

    public Task<List<ProductViewModel>> Handle(
        GetProductsRequest request,
        CancellationToken cancellationToken)
    {
        var products = _repository.GetAll();

        if (request.OnlyAvailable)
        {
            products = products
                .Where(product =>
                    product.QuantityInStock > 0 && !product.IsArchived)
                .ToList();
        }
        var viewModels =
            _mapper.Map<List<ProductViewModel>>(products);

        return Task.FromResult(viewModels);
    }
}