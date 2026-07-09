using MediatR;
using Warehouse.Domain.Interfaces;

namespace Warehouse.Application.Commands.Products.UpdateProductQuantity;

public class UpdateProductQuantityHandler : IRequestHandler<UpdateProductQuantityRequest, UpdateProductQuantityResponse>
{
    private readonly IProductRepository _repository;

    public UpdateProductQuantityHandler(IProductRepository repository)
    {
        _repository = repository;
    }

    public Task<UpdateProductQuantityResponse> Handle(UpdateProductQuantityRequest request, CancellationToken cancellationToken)
    {
        var product = _repository.GetById(request.ProductId);
        if (product == null)
        {
            return Task.FromResult(new UpdateProductQuantityResponse { Success = false });
        }
        product.UpdateQuantity(request.Quantity);
        _repository.Update(product);

        return Task.FromResult(new UpdateProductQuantityResponse { Success = true });
    }
}