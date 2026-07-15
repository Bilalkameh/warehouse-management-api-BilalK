using MediatR;
using Warehouse.Application.Exceptions;
using Warehouse.Domain.Entities;
using Warehouse.Domain.Interfaces;

namespace Warehouse.Application.Commands.Products.AddProductImage;

public class AddProductImageHandler : IRequestHandler<AddProductImageRequest, AddProductImageResponse>
{
    private readonly IProductRepository _productRepository;
    private readonly IProductImageRepository _productImageRepository;

    public AddProductImageHandler(IProductRepository productRepository, IProductImageRepository productImageRepository)
    {
        _productRepository = productRepository;
        _productImageRepository = productImageRepository;
    }

    public async Task<AddProductImageResponse> Handle(AddProductImageRequest request, CancellationToken cancellationToken)
    {
        var product = await _productRepository.GetByIdAsync(
            request.ProductId,
            cancellationToken);

        if (product == null)
            throw new NotFoundException("Product was not found.");

        var image = new ProductImage(product, request.FileName, request.FilePath);
        await _productImageRepository.AddAsync(image, cancellationToken);

        return new AddProductImageResponse
        {
            Success = true
        };
    }
}