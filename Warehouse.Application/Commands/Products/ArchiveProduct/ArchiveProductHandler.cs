using MediatR;
using Warehouse.Domain.Interfaces;
using Warehouse.Application.Exceptions;


namespace Warehouse.Application.Commands.Products.ArchiveProduct;

public class ArchiveProductHandler
    : IRequestHandler<ArchiveProductRequest, ArchiveProductResponse>
{
    private readonly IProductRepository _repository;

    public ArchiveProductHandler(IProductRepository repository)
    {
        _repository = repository;
    }

    public async Task<ArchiveProductResponse> Handle(ArchiveProductRequest request, CancellationToken cancellationToken)
    {
        var product = await _repository.GetByIdAsync(request.ProductId, cancellationToken);

        if (product == null)
            throw new NotFoundException("Product was not found.");

        product.Archive();
        
        await _repository.UpdateAsync(product, cancellationToken);

        return new ArchiveProductResponse
        {
            Success = true
        };
    }
}