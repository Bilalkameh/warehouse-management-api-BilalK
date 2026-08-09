using MediatR;
using Warehouse.Application.Exceptions;
using Warehouse.Domain.Exceptions;
using Warehouse.Domain.Interfaces;

namespace Warehouse.Application.Commands.Products.AssignSupplier;

public class AssignSupplierHandler : IRequestHandler<AssignSupplierRequest, AssignSupplierResponse>
{
    private readonly IProductRepository _productRepository;
    private readonly ISupplierRepository _supplierRepository;

    public AssignSupplierHandler(IProductRepository productRepository, ISupplierRepository supplierRepository)
    {
        _productRepository = productRepository;
        _supplierRepository = supplierRepository;
    }

    public async Task<AssignSupplierResponse> Handle(AssignSupplierRequest request, CancellationToken cancellationToken)
    {
        var product = await _productRepository.GetByIdAsync(request.ProductId, cancellationToken);

        if (product == null)
            throw new NotFoundException("Product was not found.");

        if (product.IsArchived)
            throw new BusinessRuleException("Archived products cannot be updated.");

        var supplier = await _supplierRepository.GetByIdAsync(request.SupplierId, cancellationToken);

        if (supplier == null)
            throw new NotFoundException("Supplier was not found.");

        product.AssignSupplier(supplier);

        await _productRepository.UpdateAsync(product, cancellationToken);

        return new AssignSupplierResponse
        {
            Success = true
        };
    }
}