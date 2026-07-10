using MediatR;
using Warehouse.Domain.Interfaces;

namespace Warehouse.Application.Queries.Suppliers.GetSuppliers;

public class GetSuppliersHandler : IRequestHandler<GetSuppliersRequest, List<GetSuppliersResponse>>
{
    private readonly ISupplierRepository _repository;
    
    public GetSuppliersHandler(ISupplierRepository repository)
    {
        _repository = repository;
    }

    public Task<List<GetSuppliersResponse>> Handle(GetSuppliersRequest request, CancellationToken cancellationToken)
    {
        var suppliers = _repository.GetAll();

        var response = new List<GetSuppliersResponse>();

        foreach (var supplier in suppliers)
        {
            var item = new GetSuppliersResponse();
            item.Id = supplier.Id;
            item.Name = supplier.Name;
            item.Country = supplier.Country;
            item.ContactEmail = supplier.ContactEmail;
            item.PhoneNumber = supplier.PhoneNumber;
            item.IsActive = supplier.IsActive;

            response.Add(item);
        }

        return Task.FromResult(response);
    }

}