using MediatR;
using Warehouse.Domain.Interfaces;

namespace Warehouse.Application.Queries.Suppliers.GetSupplierById;

public class GetSupplierByIdHandler : IRequestHandler<GetSupplierByIdRequest, GetSupplierByIdResponse?>
{
    private readonly ISupplierRepository _repository;
    public GetSupplierByIdHandler(ISupplierRepository repository)
    {
        _repository = repository;
    }

    public Task<GetSupplierByIdResponse?> Handle(GetSupplierByIdRequest request, CancellationToken cancellationToken)
    {
        var supplier = _repository.GetById(request.SupplierId);

        if (supplier == null)
        {
            return Task.FromResult<GetSupplierByIdResponse?>(null);
        }

        var response = new GetSupplierByIdResponse();
        response.Id = supplier.Id;
        response.Name = supplier.Name;
        response.Country = supplier.Country;
        response.ContactEmail = supplier.ContactEmail;
        response.PhoneNumber = supplier.PhoneNumber;
        response.IsActive = supplier.IsActive;

        return Task.FromResult<GetSupplierByIdResponse?>(response);
    }
}