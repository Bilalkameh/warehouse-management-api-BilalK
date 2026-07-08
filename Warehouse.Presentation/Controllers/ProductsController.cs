using Microsoft.AspNetCore.Mvc;
using Warehouse.Application.Commands.Products;
using Warehouse.Application.Handlers.Products;
using Warehouse.Application.Queries.Products;
using Warehouse.Domain.Entities;

namespace Warehouse.Presentation.Controllers;

[ApiController]
[Route("api/products")]
public class ProductsController : ControllerBase
{
    private readonly GetProductsHandler _getProductsHandler;
    private readonly GetProductByIdHandler _getProductByIdHandler;
    private readonly SearchProductsHandler _searchProductsHandler;
    private readonly CreateProductHandler _createProductHandler;
    private readonly UpdateProductQuantityHandler _updateQuantityHandler;
    private readonly UpdateProductPriceHandler _updatePriceHandler;
    private readonly ArchiveProductHandler _archiveProductHandler;
    private readonly AssignSupplierHandler _assignSupplierHandler;
    private readonly AddProductImageHandler _addProductImageHandler;


    public ProductsController(
        GetProductsHandler getProductsHandler,
        GetProductByIdHandler getProductByIdHandler,
        SearchProductsHandler searchProductsHandler,
        CreateProductHandler createProductHandler,
        UpdateProductQuantityHandler updateQuantityHandler,
        UpdateProductPriceHandler updatePriceHandler,
        ArchiveProductHandler archiveProductHandler,
        AssignSupplierHandler assignSupplierHandler,
        AddProductImageHandler addProductImageHandler)
    {
        _getProductsHandler = getProductsHandler;
        _getProductByIdHandler = getProductByIdHandler;
        _searchProductsHandler = searchProductsHandler;
        _createProductHandler = createProductHandler;
        _updateQuantityHandler = updateQuantityHandler;
        _updatePriceHandler = updatePriceHandler;
        _archiveProductHandler = archiveProductHandler;
        _assignSupplierHandler = assignSupplierHandler;
        _addProductImageHandler = addProductImageHandler;
    }


    // 1. Get all products
    [HttpGet]
    public IActionResult GetAllProducts([FromQuery] bool onlyAvailable = false)
    {
        var query = new GetProductsQuery
        {
            OnlyAvailable = onlyAvailable
        };

        var products = _getProductsHandler.Handle(query);

        return Ok(products);
    }


    // 2. Get product by id
    [HttpGet("{id}")]
    public IActionResult GetProductById([FromRoute] Guid id)
    {
        var query = new GetProductByIdQuery
        {
            ProductId = id
        };

        var product = _getProductByIdHandler.Handle(query);

        if (product == null)
            return NotFound("Product not found");

        return Ok(product);
    }


    // 3. Search products
    [HttpGet("search")]
    public IActionResult SearchProducts(
        [FromQuery] string? name,
        [FromQuery] string? supplier)
    {
        var query = new SearchProductsQuery
        {
            Name = name,
            SupplierName = supplier
        };

        var products = _searchProductsHandler.Handle(query);

        return Ok(products);
    }


    // 4. Create product
    [HttpPost]
    public IActionResult CreateProduct(
        [FromBody] CreateProductCommand command)
    {
        var product = _createProductHandler.Handle(command);

        return CreatedAtAction(
            nameof(GetProductById),
            new { id = product.Id },
            product);
    }


    // 5. Update quantity
    [HttpPost("{id}/quantity")]
    public IActionResult UpdateQuantity(
        [FromRoute] Guid id,
        [FromBody] int quantity)
    {
        var command = new UpdateProductQuantityCommand
        {
            ProductId = id,
            Quantity = quantity
        };

        var result = _updateQuantityHandler.Handle(command);

        if (!result)
            return BadRequest("Invalid product or quantity");

        return Ok();
    }


    // 6. Update price
    [HttpPost("{id}/price")]
    public IActionResult UpdatePrice(
        [FromRoute] Guid id,
        [FromBody] double price)
    {
        var command = new UpdateProductPriceCommand
        {
            ProductId = id,
            Price = price
        };

        var result = _updatePriceHandler.Handle(command);

        if (!result)
            return BadRequest("Invalid product or price");

        return Ok();
    }


    // 7. Delete product (soft delete)
    [HttpDelete("{id}")]
    public IActionResult DeleteProduct(
        [FromRoute] Guid id)
    {
        var command = new ArchiveProductCommand
        {
            ProductId = id
        };

        var result = _archiveProductHandler.Handle(command);

        if (!result)
            return NotFound("Product not found");

        return Ok();
    }
    
    // 8. Upload image
    [HttpPost("{id}/image")]
    public IActionResult UploadImage(
        [FromRoute] Guid id,
        IFormFile? file)
    {
        if(file == null || file.Length == 0)
            return BadRequest("No file was uploaded.");


        var command = new AddProductImageCommand
        {
            ProductId = id,
            FileName = file.FileName,
            FilePath = $"uploads/{file.FileName}"
        };


        var result = _addProductImageHandler.Handle(command);


        if(result == null)
            return BadRequest("Invalid product or file.");


        return Ok(result);
    }


    // 9. Server time
    [HttpGet("server-time")]
    public IActionResult GetServerTime(
        [FromHeader(Name = "Accept-Language")] string? language)
    {
        var now = DateTime.UtcNow;

        var culture = language switch
        {
            "fr-FR" => new System.Globalization.CultureInfo("fr-FR"),
            "ar-LB" => new System.Globalization.CultureInfo("ar-LB"),
            _ => new System.Globalization.CultureInfo("en-US")
        };

        return Ok(new
        {
            serverTime = now.ToString("F", culture)
        });
    }


    // 10. Assign supplier
    [HttpPost("{id}/assign-supplier/{supplierId}")]
    public IActionResult AssignSupplier(
        [FromRoute] Guid id,
        [FromRoute] Guid supplierId)
    {
        var command = new AssignSupplierCommand
        {
            ProductId = id,
            SupplierId = supplierId
        };

        var result = _assignSupplierHandler.Handle(command);

        if (!result)
            return BadRequest("Invalid product or supplier");

        return Ok();
    }
    
}