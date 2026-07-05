using warehouse_management.Models;
using warehouse_management.Contracts;
namespace warehouse_management;

public class ProductService
{
    private readonly FakeWarehouseStore _warehouseStore;

    public ProductService(FakeWarehouseStore warehouseStore)
    {
        _warehouseStore = warehouseStore;
    }
    // ProductService contains the business logic for product-related operations.
    // All filtering, validation rules, and product-related decision logic have been
    // moved out of the controller layer into this service as part of the required Controller-Service separation refactor.
    
    
    // 1. Get All Products 
    public List<Product> GetAllProducts(bool onlyAvailable)
    {
        var products = _warehouseStore.GetAll();

        products = products.Where(p => !p.IsArchived).ToList();

        if (onlyAvailable)
        {
            products = products.Where(p => p.QuantityInStock > 0).ToList();
        }

        products = products
            .OrderByDescending(p => p.CreatedAt)
            .ToList();

        return products;
    }
    
    // 2. Get Product by Id
    public Product? GetProductById(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
            return null;

        var product = _warehouseStore.GetById(id);
        if (product == null || product.IsArchived)
            return null;

        return product;
    }
    
    // 3. Search Product 
    
    public List<Product>? SearchProducts(string? name, string? supplier)
    {
        if (string.IsNullOrWhiteSpace(name) && string.IsNullOrWhiteSpace(supplier))
            return null;

        var products = _warehouseStore.Search(name, supplier)
            .Where(p => !p.IsArchived)
            .ToList();

        return products;
    }
    
    // 4. Create Product 
    public Product? CreateProduct(CreateProductRequest request)
    {
        if (request == null)
            throw new ArgumentNullException(nameof(request));

        //There is actually no point in this check because I never assigned SKU to the existing products
        //Only way this may trigger is if the user tries to add two products with the same SKU that he assigns
        
        foreach (var product in _warehouseStore.GetAll())
        {   
            
            if (!string.IsNullOrWhiteSpace(product.SKU) &&
                product.SKU.Equals(request.SKU, StringComparison.OrdinalIgnoreCase))
            {
                return null;
            }
        }

        var newProduct = new Product(Guid.NewGuid().ToString(), request.Name, request.Description, 
            request.Price, request.QuantityInStock, request.SupplierName, request.ExpiryDate);

        newProduct.SKU = request.SKU;
        _warehouseStore.Add(newProduct);

        return newProduct;
    }
    
    //5. Update Quantity
    
    public bool UpdateQuantity(string id, int quantity)
    {
        if (string.IsNullOrWhiteSpace(id) || quantity < 0)
            return false;

        var product = _warehouseStore.GetById(id);

        if (product == null || product.IsArchived)
            return false;

        product.QuantityInStock = quantity;
        product.LastUpdatedAt = DateTime.UtcNow;
        return true;
    }
    
    //6. Update price
    
    public bool UpdatePrice(string id, double price)
    {
        if (string.IsNullOrWhiteSpace(id) || price <= 0)
            return false;

        var product = _warehouseStore.GetById(id);

        if (product == null || product.IsArchived)
            return false;

        product.Price = price;
        product.LastUpdatedAt = DateTime.UtcNow;

        return true;
    }
    
    //7. Delete Product
    
    public bool DeleteProduct(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
            return false;

        var product = _warehouseStore.GetById(id);

        if (product == null || product.IsArchived)
            return false;

        product.IsArchived = true;
        product.LastUpdatedAt = DateTime.UtcNow;
        return true;
    }
    
    //8. Assign Supplier to Product
    public Product? AssignSupplier(string productId, string supplierId, FakeSupplierStore supplierStore)
    {
        if (string.IsNullOrWhiteSpace(productId) || string.IsNullOrWhiteSpace(supplierId))
            return null;

        var product = _warehouseStore.GetById(productId);

        if (product == null || product.IsArchived)
            return null;

        var supplier = supplierStore.GetSupplierById(supplierId);

        if (supplier == null || !supplier.IsActive)
            return null;

        if (product.SupplierName == supplier.Name)
            return null;

        product.SupplierName = supplier.Name;
        product.LastUpdatedAt = DateTime.UtcNow;
        return product;
    }
    
    //9. Upload Image
    
    public ProductImage? AddProductImage(string id, IFormFile file)
    {
        var product = _warehouseStore.GetById(id);

        if (product == null || product.IsArchived)
            return null;

        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();

        if (!allowedExtensions.Contains(extension))
            return null;

        const long maxSizeBytes = 2 * 1024 * 1024;
        if (file.Length > maxSizeBytes)
            return null;

        var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
        Directory.CreateDirectory(uploadsFolder);

        var fileName = $"{Guid.NewGuid()}{extension}";
        var filePath = Path.Combine(uploadsFolder, fileName);

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            file.CopyTo(stream);
        }

        product.LastUpdatedAt = DateTime.UtcNow;
        return new ProductImage
            
        {
            ProductId = product.Id,
            FileName = fileName,
            FilePath = $"/uploads/{fileName}"
        };
    }
}
