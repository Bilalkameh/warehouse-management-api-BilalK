using AutoMapper;
using MediatR;
using Warehouse.Application.ViewModels;
using Warehouse.Domain.Entities;
using Warehouse.Domain.Interfaces;

namespace Warehouse.Application.Commands.Products.CreateProduct;

public class CreateProductHandler
    : IRequestHandler<CreateProductRequest, ProductViewModel>
{
    private readonly IProductRepository _productRepository;
    private readonly ISupplierRepository _supplierRepository;
    private readonly IMapper _mapper;

    public CreateProductHandler(
        IProductRepository productRepository,
        ISupplierRepository supplierRepository,
        IMapper mapper)
    {
        _productRepository = productRepository;
        _supplierRepository = supplierRepository;
        _mapper = mapper;
    }

    public Task<ProductViewModel> Handle(
        CreateProductRequest request,
        CancellationToken cancellationToken)
    {
        var supplier = _supplierRepository
            .GetAll()
            .FirstOrDefault(supplier =>
                supplier.Name.Equals(
                    request.SupplierName,
                    StringComparison.OrdinalIgnoreCase));

        if (supplier == null)
            throw new Exception("Supplier not found.");

        var product = new Product(
            request.Name,
            request.SKU,
            request.Description,
            request.Price,
            request.QuantityInStock,
            supplier,
            request.ExpiryDate);

        _productRepository.Add(product);
        var viewModel = _mapper.Map<ProductViewModel>(product);

        return Task.FromResult(viewModel);
    }
}