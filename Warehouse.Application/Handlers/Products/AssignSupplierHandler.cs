using Warehouse.Application.Commands.Products;
using Warehouse.Domain.Interfaces;

namespace Warehouse.Application.Handlers.Products;

public class AssignSupplierHandler
{
    private readonly IProductRepository _productRepository;
    private readonly ISupplierRepository _supplierRepository;

    public AssignSupplierHandler(
        IProductRepository productRepository,
        ISupplierRepository supplierRepository)
    {
        _productRepository = productRepository;
        _supplierRepository = supplierRepository;
    }


    public bool Handle(AssignSupplierCommand command)
    {
        var product = _productRepository.GetById(command.ProductId);
        var supplier = _supplierRepository.GetById(command.SupplierId);
        
        if(product == null || supplier == null)
            return false;

	try
	{
    	product.AssignSupplier(supplier);
    	_productRepository.Update(product);

   	 return true;
	}
	catch(Exception)
	{
    	return false;
	}
    }
}