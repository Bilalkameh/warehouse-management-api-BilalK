using MediatR;
using Warehouse.Domain.Interfaces;
using Warehouse.Application.Exceptions;


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
        var product = await _repository.GetByIdAsync(request.ProductId, cancellationToken);

        if (product == null)
            throw new NotFoundException("Product was not found.");

        product.UpdatePrice(request.Price);


        await _repository.UpdateAsync(product, cancellationToken);

        return new UpdateProductPriceResponse
        {
            Success = true
        };
    }
}