using Warehouse.Application.Commands.Products;
using Warehouse.Domain.Entities;
using Warehouse.Domain.Interfaces;

namespace Warehouse.Application.Handlers.Products;

public class AddProductImageHandler
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
    
    public ProductImage? Handle(AddProductImageCommand command)
    {
        var product = _productRepository.GetById(command.ProductId);
        if(product == null)
            return null;
        
        var image = new ProductImage(
            command.ProductId,
            command.FileName,
            command.FilePath
        );
        
        _imageRepository.Add(image);
        return image;
    }
}