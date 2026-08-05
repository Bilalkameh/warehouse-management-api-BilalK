Context: There is currently a business-logic defect in the AssignSupplier implementation located under Commands/Products/AssignSupplier in the Application layer.

An archived product can still be assigned to an active supplier, even though archived products must not be modified.

AssignSupplierHandler currently contains:
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

Task: Analyze and refactor the AssignSupplier flow so that a supplier cannot be assigned to an archived product.

Requirements:
- A clear plain-text explanation of the faulty logic sequence.
- The precise line numbers or code blocks containing the logical oversight. Do not invent line numbers if they cannot be determined from the provided code.
- A robust and efficient C# fix that respects the existing domain rules, architecture, naming conventions, and exception patterns.
- Perform validation in an efficient order and avoid unnecessary repository calls.
- An isolated unit test under the existing tests folder verifying that assigning a supplier to an archived product throws the appropriate exception.
- Verify in the test that the archived product is not updated or persisted after the exception.

Constraints: 
- Do not introduce new packages, unrelated refactors, or architectural abstractions.