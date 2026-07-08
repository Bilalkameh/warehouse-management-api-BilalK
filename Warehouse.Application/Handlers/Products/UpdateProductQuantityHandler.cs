using Warehouse.Application.Commands.Products;
using Warehouse.Domain.Interfaces;

namespace Warehouse.Application.Handlers.Products;

public class UpdateProductQuantityHandler
{
    private readonly IProductRepository _repository;

    public UpdateProductQuantityHandler(IProductRepository repository)
    {
        _repository = repository;
    }

    public bool Handle(UpdateProductQuantityCommand command)
    {
        var product = _repository.GetById(command.ProductId);

        if (product == null)
            return false;

        product.UpdateQuantity(command.Quantity);
        _repository.Update(product);

        return true;
    }
}