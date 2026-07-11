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

    public Task<UpdateProductPriceResponse> Handle(UpdateProductPriceRequest request, CancellationToken cancellationToken)
    {
        var product = _repository.GetById(request.ProductId);
        if (product == null)
        {
            return Task.FromResult(new UpdateProductPriceResponse { Success = false });
        }
        product.Price = request.Price;
        product.LastUpdatedAt = DateTime.UtcNow;
        _repository.Update(product);
        
        return Task.FromResult(new UpdateProductPriceResponse { Success = true });
    }
}