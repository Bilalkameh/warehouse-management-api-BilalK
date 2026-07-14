namespace Warehouse.Domain.Entities;

public class ProductImage
{
    public Guid Id { get; private set; }
    public Guid ProductId { get; private set; }
    public Product Product { get; private set; } = null!;
    public string FileName { get; private set; } = string.Empty;
    public string FilePath { get; private set; } = string.Empty;

//This constructor ensures that a ProductImage cannot be created without the required information.
// File existence and upload validation remain outside the Domain layer.

    public ProductImage(
        Product product,
        string fileName,
        string filePath)
    {
        if (product == null)
            throw new ArgumentNullException(nameof(product));

        if (string.IsNullOrWhiteSpace(fileName))
            throw new Exception("File name is required.");

        if (string.IsNullOrWhiteSpace(filePath))
            throw new Exception("File path is required.");

        Id = Guid.NewGuid();
        Product = product;
        ProductId = product.Id;
        FileName = fileName;
        FilePath = filePath;
    }
    
    private ProductImage()
    {
    }
}