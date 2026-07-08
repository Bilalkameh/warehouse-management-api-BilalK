using Warehouse.Application.Queries.Suppliers;
using Warehouse.Domain.Entities;
using Warehouse.Domain.Interfaces;


namespace Warehouse.Application.Handlers.Suppliers;


public class GetSuppliersHandler
{
    private readonly ISupplierRepository _repository;
    public GetSuppliersHandler(ISupplierRepository repository)
    {
        _repository = repository;
    }
    
    public List<Supplier> Handle(GetSuppliersQuery query)
    {
        return _repository.GetAll();
    }
}