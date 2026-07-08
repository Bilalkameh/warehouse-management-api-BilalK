using Warehouse.Application.Commands.Products;
using Warehouse.Domain.Interfaces;

namespace Warehouse.Application.Handlers.Products;

public class ArchiveProductHandler
{
    private readonly IProductRepository _repository;
    public ArchiveProductHandler(IProductRepository repository)
    {
        _repository = repository;
    }

    public bool Handle(ArchiveProductCommand command)
    {
        var product = _repository.GetById(command.ProductId);

        if(product == null)
            return false;
        
        product.Archive();
        _repository.Update(product);

        return true;
    }
}