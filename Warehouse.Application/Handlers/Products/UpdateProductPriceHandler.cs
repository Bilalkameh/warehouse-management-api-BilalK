using Warehouse.Application.Commands.Products;
using Warehouse.Domain.Interfaces;

namespace Warehouse.Application.Handlers.Products;

public class UpdateProductPriceHandler
{
    private readonly IProductRepository _repository;

    public UpdateProductPriceHandler(IProductRepository repository)
    {
        _repository = repository;
    }

    public bool Handle(UpdateProductPriceCommand command)
    {
        var product = _repository.GetById(command.ProductId);

        if (product == null)
            return false;

        product.UpdatePrice(command.Price);
        _repository.Update(product);

        return true;
    }
}