using Warehouse.Application.Commands.Suppliers;
using Warehouse.Domain.Interfaces;


namespace Warehouse.Application.Handlers.Suppliers;


public class DeactivateSupplierHandler
{
    private readonly ISupplierRepository _repository;


    public DeactivateSupplierHandler(ISupplierRepository repository)
    {
        _repository = repository;
    }


    public bool Handle(DeactivateSupplierCommand command)
    {
        var supplier = _repository.GetById(command.SupplierId);


        if(supplier == null)
            return false;
        
        supplier.Deactivate();
        
        _repository.Update(supplier);
        
        return true;
    }
}