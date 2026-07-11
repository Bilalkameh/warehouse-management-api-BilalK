using MediatR;
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

    public Task<AssignSupplierResponse> Handle(AssignSupplierRequest request, CancellationToken cancellationToken)
    {
        var product = _productRepository.GetById(request.ProductId);
        var supplier = _supplierRepository.GetById(request.SupplierId);

        if (product == null || supplier == null)
        {
            var failedResponse = new AssignSupplierResponse();
            failedResponse.Success = false;

            return Task.FromResult(failedResponse);
        }

        if (product.IsArchived || !supplier.IsActive)
        {
            var failedResponse = new AssignSupplierResponse();
            failedResponse.Success = false;

            return Task.FromResult(failedResponse);
        }

        product.SupplierId = supplier.Id;
        product.Supplier = supplier;
        product.LastUpdatedAt = DateTime.UtcNow;
        _productRepository.Update(product);
        var response = new AssignSupplierResponse();
        response.Success = true;

        return Task.FromResult(response);
    }

}