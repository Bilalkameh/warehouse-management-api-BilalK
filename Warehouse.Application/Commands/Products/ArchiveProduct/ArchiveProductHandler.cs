using MediatR;
using Warehouse.Domain.Interfaces;

namespace Warehouse.Application.Commands.Products.ArchiveProduct;

public class ArchiveProductHandler : IRequestHandler<ArchiveProductRequest, ArchiveProductResponse>
{
    private readonly IProductRepository _repository;

    public ArchiveProductHandler(IProductRepository repository)
    {
        _repository = repository;
    }

    public Task<ArchiveProductResponse> Handle(ArchiveProductRequest request, CancellationToken cancellationToken)
    {
        var product = _repository.GetById(request.ProductId);

        if (product == null)
        {
            return Task.FromResult(new ArchiveProductResponse { Success = false });
        }
        product.IsArchived = true;
        product.LastUpdatedAt = DateTime.UtcNow;
        _repository.Update(product);

        return Task.FromResult(new ArchiveProductResponse { Success = true });
    }
}