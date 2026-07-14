using MediatR;
using Warehouse.Domain.Interfaces;

namespace Warehouse.Application.Commands.Suppliers.DeactivateSupplier;

public class DeactivateSupplierHandler
    : IRequestHandler<DeactivateSupplierRequest, DeactivateSupplierResponse>
{
    private readonly ISupplierRepository _repository;

    public DeactivateSupplierHandler(ISupplierRepository repository)
    {
        _repository = repository;
    }

    public async Task<DeactivateSupplierResponse> Handle(DeactivateSupplierRequest request, CancellationToken cancellationToken)
    {
        var supplier = await _repository.GetByIdAsync(request.SupplierId, cancellationToken);

        if (supplier == null)
        {
            return new DeactivateSupplierResponse
            {
                Success = false
            };
        }

        supplier.Deactivate();
        await _repository.UpdateAsync(supplier, cancellationToken);
        
        return new DeactivateSupplierResponse
        {
            Success = true
        };
    }
}