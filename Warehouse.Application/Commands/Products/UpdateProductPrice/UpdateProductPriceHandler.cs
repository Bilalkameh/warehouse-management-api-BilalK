using MediatR;
using Warehouse.Domain.Interfaces;

namespace Warehouse.Application.Commands.Products.UpdateProductPrice;

public class UpdateProductPriceHandler : IRequestHandler<UpdateProductPriceRequest, UpdateProductPriceResponse>
{
    private readonly IProductRepository _repository;

    public UpdateProductPriceHandler(IProductRepository repository)
    {
        _repository = repository;
    }

    public async Task<UpdateProductPriceResponse> Handle(UpdateProductPriceRequest request, CancellationToken cancellationToken)
    {
        var product = await _repository.GetByIdAsync(
            request.ProductId,
            cancellationToken);

        if (product == null)
        {
            return new UpdateProductPriceResponse
            {
                Success = false
            };
        }

        try
        {
            product.UpdatePrice(request.Price);
        }
        catch (Exception)
        {
            return new UpdateProductPriceResponse
            {
                Success = false
            };
        }

        await _repository.UpdateAsync(product, cancellationToken);

        return new UpdateProductPriceResponse
        {
            Success = true
        };
    }
}