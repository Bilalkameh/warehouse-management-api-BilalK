using MediatR;
using Microsoft.AspNetCore.Mvc;
using Warehouse.Application.Commands.Products.AddProductImage;
using Warehouse.Application.Commands.Products.ArchiveProduct;
using Warehouse.Application.Commands.Products.AssignSupplier;
using Warehouse.Application.Commands.Products.CreateProduct;
using Warehouse.Application.Commands.Products.UpdateProductPrice;
using Warehouse.Application.Commands.Products.UpdateProductQuantity;
using Warehouse.Application.Queries.Products.GetProductById;
using Warehouse.Application.Queries.Products.GetProducts;
using Warehouse.Application.Queries.Products.SearchProducts;
using System.ComponentModel.DataAnnotations;

namespace Warehouse.Presentation.Controllers;

[ApiController]
[Route("api/products")]
public class ProductsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ProductsController(IMediator mediator)
    
    {
        _mediator = mediator;   
    }


    // 1. Get all products
    [HttpGet]
    public async Task<IActionResult> GetAllProducts([FromQuery] bool onlyAvailable = false, CancellationToken cancellationToken = default)
    {
        var request = new GetProductsRequest
        {
            OnlyAvailable = onlyAvailable
        };

        var products = await _mediator.Send(request, cancellationToken);
        return Ok(products);
    }

    // 2. Get product by id
    [HttpGet("{id}")]
    public async Task<IActionResult> GetProductById([FromRoute] Guid id, CancellationToken cancellationToken )
    {
        var request = new GetProductByIdRequest
        {
            ProductId = id
        };
        var product = await _mediator.Send(request, cancellationToken);

        return Ok(product);
    }


    // 3. Search products
    [HttpGet("search")]
    public async Task<IActionResult> SearchProducts([FromQuery] string? name, [FromQuery] string? supplier, CancellationToken cancellationToken)
    {
        var request = new SearchProductsRequest
        {
            Name = name,
            SupplierName = supplier
        };
        var products = await _mediator.Send(request, cancellationToken);
        
        return Ok(products);
    }


    // 4. Create product
    [HttpPost]
    public async Task<IActionResult> CreateProduct([FromBody] CreateProductRequest request, CancellationToken cancellationToken)
    {
        var product = await _mediator.Send(request, cancellationToken);

        return CreatedAtAction(
            nameof(GetProductById),
            new { id = product.Id },
            product);
    }


    // 5. Update quantity
    [HttpPost("{id}/quantity")]
    public async Task<IActionResult> UpdateQuantity([FromRoute] Guid id, 
        [FromBody] [Range(0, int.MaxValue, ErrorMessage = "Quantity cannot be negative.")] int quantity,
        CancellationToken cancellationToken = default)
    {
        var request = new UpdateProductQuantityRequest
        {
            ProductId = id,
            Quantity = quantity
        };

        await _mediator.Send(request, cancellationToken);
        
        return Ok();
    }


    // 6. Update price
    [HttpPost("{id}/price")]
    public async Task<IActionResult> UpdatePrice([FromRoute] Guid id, [FromBody] 
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than zero.")] double price,
        CancellationToken cancellationToken)
    {    
        var request = new UpdateProductPriceRequest
        {
            ProductId = id,
            Price = price
        };

        await _mediator.Send(request, cancellationToken);

        return Ok();
    }


    // 7. Delete product (soft delete)
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteProduct([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var request = new ArchiveProductRequest
        {
            ProductId = id
        };

        await _mediator.Send(request, cancellationToken);

        return Ok();
    }
    
    // 8. Upload image
    [HttpPost("{id}/image")]
    public async Task<IActionResult> UploadImage([FromRoute] Guid id, IFormFile? file, CancellationToken cancellationToken)
    {
        if (file == null || file.Length == 0)
            return BadRequest("No file was uploaded.");

        var request = new AddProductImageRequest
        {
            ProductId = id,
            FileName = file.FileName,
            FilePath = $"uploads/{file.FileName}"
        };

        var result = await _mediator.Send(request, cancellationToken);

        return Ok(result);
    }


    // 9. Server time
    [HttpGet("server-time")]
    public IActionResult GetServerTime([FromHeader(Name = "Accept-Language")] string? language)
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
    public async Task<IActionResult> AssignSupplier([FromRoute] Guid id, [FromRoute] Guid supplierId, CancellationToken cancellationToken)
    {
        var request = new AssignSupplierRequest
        {
            ProductId = id,
            SupplierId = supplierId
        };

        await _mediator.Send(request, cancellationToken);

        return Ok();
    }
    
}