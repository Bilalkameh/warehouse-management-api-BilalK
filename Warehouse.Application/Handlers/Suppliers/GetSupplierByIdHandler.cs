using Warehouse.Application.Queries.Suppliers;
using Warehouse.Domain.Entities;
using Warehouse.Domain.Interfaces;


namespace Warehouse.Application.Handlers.Suppliers;


public class GetSupplierByIdHandler
{
    private readonly ISupplierRepository _repository;
    public GetSupplierByIdHandler(ISupplierRepository repository)
    {
        _repository = repository;
    }
    
    public Supplier? Handle(GetSupplierByIdQuery query)
    {
        return _repository.GetById(query.SupplierId);
    }
}