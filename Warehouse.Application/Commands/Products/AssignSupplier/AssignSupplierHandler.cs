using MediatR;
using Warehouse.Domain.Interfaces;
using Warehouse.Application.Exceptions;

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

        var supplier = await _supplierRepository.GetByIdAsync(request.SupplierId, cancellationToken);

        if (product == null)
            throw new NotFoundException("Product was not found.");

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