using MediatR;
using Warehouse.Domain.Entities;
using Warehouse.Domain.Interfaces;

namespace Warehouse.Application.Commands.Products.AddProductImage;

public class AddProductImageHandler : IRequestHandler<AddProductImageRequest, AddProductImageResponse>
{
    private readonly IProductRepository _productRepository;
    private readonly IProductImageRepository _imageRepository;

    public AddProductImageHandler(
        IProductRepository productRepository,
        IProductImageRepository imageRepository)
    {
        _productRepository = productRepository;
        _imageRepository = imageRepository;
    }

    public Task<AddProductImageResponse> Handle(
        AddProductImageRequest request,
        CancellationToken cancellationToken)
    {
        var product = _productRepository.GetById(request.ProductId);

        if (product == null)
        {
            var failedResponse = new AddProductImageResponse();
            failedResponse.Success = false;

            return Task.FromResult(failedResponse);
        }

        var image = new ProductImage
        {
            Id = Guid.NewGuid(),
            ProductId = request.ProductId,
            FileName = request.FileName,
            FilePath = request.FilePath
        };

        _imageRepository.Add(image);
        var response = new AddProductImageResponse();
        response.Id = image.Id;
        response.Success = true;

        return Task.FromResult(response);
    }
}