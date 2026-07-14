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

    public async Task<UpdateProductQuantityResponse> Handle(UpdateProductQuantityRequest request, CancellationToken cancellationToken)
    {
        var product = await _repository.GetByIdAsync(
            request.ProductId,
            cancellationToken);

        if (product == null)
        {
            return new UpdateProductQuantityResponse
            {
                Success = false
            };
        }

        try
        {
            product.UpdateQuantity(request.Quantity);
        }
        catch (Exception)
        {
            return new UpdateProductQuantityResponse
            {
                Success = false
            };
        }

        await _repository.UpdateAsync(product, cancellationToken);

        return new UpdateProductQuantityResponse
        {
            Success = true
        };
    }
}