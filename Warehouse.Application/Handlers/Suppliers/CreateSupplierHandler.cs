using Warehouse.Application.Commands.Suppliers;
using Warehouse.Domain.Entities;
using Warehouse.Domain.Interfaces;


namespace Warehouse.Application.Handlers.Suppliers;


public class CreateSupplierHandler
{
    private readonly ISupplierRepository _repository;
    
    public CreateSupplierHandler(ISupplierRepository repository)
    {
        _repository = repository;
    }

    public Supplier Handle(CreateSupplierCommand command)
    {
        var supplier = new Supplier(
            command.Name,
            command.Country,
            command.ContactEmail,
            command.PhoneNumber
        );
        
        _repository.Add(supplier);

        return supplier;
    }
}