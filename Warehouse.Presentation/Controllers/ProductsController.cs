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
using Warehouse.Application.Queries.Products.GetPagedProducts;
using Warehouse.Application.Queries.Products.GetProductsBySupplier;
using Warehouse.Application.Queries.Products.GetTotalProductCount;
using Warehouse.Application.Queries.Products.GroupProductsByExpiryYear;
using Warehouse.Application.Queries.Products.GroupProductsByExpiryYearAndCountry;
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
    public async Task<IActionResult> GetAllProducts([FromQuery] bool onlyAvailable = false)
    {
        var request = new GetProductsRequest
        {
            OnlyAvailable = onlyAvailable
        };
        var products = await _mediator.Send(request);

        return Ok(products);
    }


    // 2. Get product by id
    [HttpGet("{id}")]
    public async Task<IActionResult> GetProductById([FromRoute] Guid id)
    {
        var request = new GetProductByIdRequest
        {
            ProductId = id
        };
        var product = await _mediator.Send(request);
        if (product == null)
            return NotFound("Product not found");

        return Ok(product);
    }


    // 3. Search products
    [HttpGet("search")]
    public async Task<IActionResult> SearchProducts([FromQuery] string? name, [FromQuery] string? supplier)
    {
        var request = new SearchProductsRequest
        {
            Name = name,
            SupplierName = supplier
        };
        var products = await _mediator.Send(request);
        
        return Ok(products);
    }


    // 4. Create product
    [HttpPost]
    public async Task<IActionResult> CreateProduct([FromBody] CreateProductRequest request)
    {
        var product = await _mediator.Send(request);

        return CreatedAtAction(
            nameof(GetProductById),
            new { id = product.Id },
            product);
    }


    // 5. Update quantity
    [HttpPost("{id}/quantity")]
    public async Task<IActionResult> UpdateQuantity(
        [FromRoute] Guid id,
        [FromBody] int quantity)
    {
        var request = new UpdateProductQuantityRequest
        {
            ProductId = id,
            Quantity = quantity
        };

        var result = await _mediator.Send(request);

        if (!result.Success)
            return BadRequest("Invalid product or quantity");

        return Ok();
    }


    // 6. Update price
    [HttpPost("{id}/price")]
    public async Task<IActionResult> UpdatePrice(
        [FromRoute] Guid id,
        [FromBody] double price)
    {
        var request = new UpdateProductPriceRequest
        {
            ProductId = id,
            Price = price
        };

        var result = await _mediator.Send(request);

        if (!result.Success)
            return BadRequest("Invalid product or price");

        return Ok();
    }


    // 7. Delete product (soft delete)
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteProduct(
        [FromRoute] Guid id)
    {
        var request = new ArchiveProductRequest
        {
            ProductId = id
        };

        var result = await _mediator.Send(request);

        if (!result.Success)
            return NotFound("Product not found");

        return Ok();
    }
    
    // 8. Upload image
    [HttpPost("{id}/image")]
    public async Task<IActionResult> UploadImage(
        [FromRoute] Guid id,
        IFormFile? file)
    {
        if (file == null || file.Length == 0)
            return BadRequest("No file was uploaded.");

        var request = new AddProductImageRequest
        {
            ProductId = id,
            FileName = file.FileName,
            FilePath = $"uploads/{file.FileName}"
        };

        var result = await _mediator.Send(request);
        if (!result.Success)
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
    public async Task<IActionResult> AssignSupplier(
        [FromRoute] Guid id,
        [FromRoute] Guid supplierId)
    {
        var request = new AssignSupplierRequest
        {
            ProductId = id,
            SupplierId = supplierId
        };

        var result = await _mediator.Send(request);
        if (!result.Success)
            return BadRequest("Invalid product or supplier");

        return Ok();
    }
    
    // 11. Get products by supplier
    [HttpGet("by-supplier")]
    public async Task<IActionResult> GetProductsBySupplier(
        [FromQuery] string supplierName,
        [FromQuery] bool ascending = true)
    {
        var request = new GetProductsBySupplierRequest
        {
            SupplierName = supplierName,
            Ascending = ascending
        };
        var products = await _mediator.Send(request);

        return Ok(products);
    }
    
    // 12. Group products by expiry year
    [HttpGet("group-by-expiry-year")]
    public async Task<IActionResult> GroupProductsByExpiryYear()
    {
        var request = new GroupProductsByExpiryYearRequest();
        var result = await _mediator.Send(request);

        return Ok(result);
    }
    
    
    // 13. Group products by expiry year and supplier country
    [HttpGet("group-by-expiry-year-and-country")]
    public async Task<IActionResult> GroupProductsByExpiryYearAndCountry()
    {
        var request = new GroupProductsByExpiryYearAndCountryRequest();
        var result = await _mediator.Send(request);

        return Ok(result);
    }
    
    // 14. Get total product count
    [HttpGet("count")]
    public async Task<IActionResult> GetTotalProductCount()
    {
        var request = new GetTotalProductCountRequest();
        var result = await _mediator.Send(request);

        return Ok(result);
    }
    
    // 15. Get paged products
    [HttpGet("paged")]
    public async Task<IActionResult> GetPagedProducts(
        [FromQuery] int pageSize,
        [FromQuery] int pageNumber)
    {
        if (pageSize <= 0 || pageNumber <= 0)
            return BadRequest("Page size and page number must be greater than zero.");

        var request = new GetPagedProductsRequest
        {
            PageSize = pageSize,
            PageNumber = pageNumber
        };
        var products = await _mediator.Send(request);

        return Ok(products);
    }
    
    
    
    
    
}