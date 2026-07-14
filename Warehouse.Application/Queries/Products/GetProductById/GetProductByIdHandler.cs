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

    public async Task<ProductViewModel?> Handle(GetProductByIdRequest request, CancellationToken cancellationToken)
    {
        var product = await _repository.GetByIdAsync(request.ProductId, cancellationToken);

        if (product == null)
            return null;
        
        return _mapper.Map<ProductViewModel>(product);
    }
}