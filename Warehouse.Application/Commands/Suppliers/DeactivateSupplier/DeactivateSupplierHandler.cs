using MediatR;
using Warehouse.Domain.Interfaces;

namespace Warehouse.Application.Commands.Suppliers.DeactivateSupplier;

public class DeactivateSupplierHandler : IRequestHandler<DeactivateSupplierRequest, DeactivateSupplierResponse>
{
    private readonly ISupplierRepository _repository;

    public DeactivateSupplierHandler(ISupplierRepository repository)
    {
        _repository = repository;
    }

    public Task<DeactivateSupplierResponse> Handle(DeactivateSupplierRequest request, CancellationToken cancellationToken)
    {
        var supplier = _repository.GetById(request.SupplierId);

        if (supplier == null)
        {
            var failedResponse = new DeactivateSupplierResponse();
            failedResponse.Success = false;

            return Task.FromResult(failedResponse);
        }

        supplier.IsActive = false;
        supplier.LastUpdatedAt = DateTime.UtcNow;
        _repository.Update(supplier);

        var response = new DeactivateSupplierResponse();
        response.Success = true;

        return Task.FromResult(response);
    }

}