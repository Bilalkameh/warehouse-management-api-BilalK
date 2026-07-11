using AutoMapper;
using MediatR;
using Warehouse.Application.ViewModels;
using Warehouse.Domain.Interfaces;

namespace Warehouse.Application.Queries.Products.GetProductById;

public class GetProductByIdHandler
    : IRequestHandler<GetProductByIdRequest, ProductViewModel?>
{
    private readonly IProductRepository _repository;
    private readonly IMapper _mapper;

    public GetProductByIdHandler(
        IProductRepository repository,
        IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public Task<ProductViewModel?> Handle(
        GetProductByIdRequest request,
        CancellationToken cancellationToken)
    {
        var product = _repository.GetById(request.ProductId);

        if (product == null)
            return Task.FromResult<ProductViewModel?>(null);

        var viewModel = _mapper.Map<ProductViewModel>(product);
        return Task.FromResult<ProductViewModel?>(viewModel);
    }
}