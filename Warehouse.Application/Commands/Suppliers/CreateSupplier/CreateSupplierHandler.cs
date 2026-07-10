using MediatR;
using Warehouse.Domain.Entities;
using Warehouse.Domain.Interfaces;

namespace Warehouse.Application.Commands.Suppliers.CreateSupplier;

public class CreateSupplierHandler : IRequestHandler<CreateSupplierRequest, CreateSupplierResponse>
{
    private readonly ISupplierRepository _repository;
    
    public CreateSupplierHandler(ISupplierRepository repository)
    {
        _repository = repository;
    }

    public Task<CreateSupplierResponse> Handle(CreateSupplierRequest request, CancellationToken cancellationToken)
    {
        var supplier = new Supplier(request.Name,
            request.Country,
            request.ContactEmail,
            request.PhoneNumber
        );

        _repository.Add(supplier);

        var response = new CreateSupplierResponse();
        response.Id = supplier.Id;
        response.Name = supplier.Name;
        response.Country = supplier.Country;
        response.ContactEmail = supplier.ContactEmail;
        response.PhoneNumber = supplier.PhoneNumber;
        response.IsActive = supplier.IsActive;

        return Task.FromResult(response);
    }
}