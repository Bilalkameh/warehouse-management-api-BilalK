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
using Warehouse.Presentation.Resources;
using Microsoft.AspNetCore.Authorization;
using Warehouse.Presentation.Authorization;
using Warehouse.Application.Commands.Products.DeleteProductImage;
using Warehouse.Application.Commands.Products.ReplaceProductImage;
using Warehouse.Application.Queries.Products.DownloadProductImage;
using Warehouse.Application.Queries.Products.GetProductImages;

namespace Warehouse.Presentation.Controllers;

[ApiController]
[Route("api/products")]
[Authorize(Policy = AuthorizationPolicies.User)]
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
    [Authorize(Policy = AuthorizationPolicies.Admin)]
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
    [Authorize(Policy = AuthorizationPolicies.Admin)]
    public async Task<IActionResult> UpdateQuantity(
        [FromRoute] Guid id,
        [FromBody]
        [Range(0, int.MaxValue,
            ErrorMessageResourceType = typeof(SharedResources),
            ErrorMessageResourceName = nameof(SharedResources.QuantityCannotBeNegative))]
        int quantity, CancellationToken cancellationToken = default)
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
    [Authorize(Policy = AuthorizationPolicies.Admin)]
    public async Task<IActionResult> UpdatePrice(
        [FromRoute] Guid id,
        [FromBody]
        [Range(0.01, double.MaxValue,
            ErrorMessageResourceType = typeof(SharedResources),
            ErrorMessageResourceName = nameof(SharedResources.PriceMustBeGreaterThanZero))]
        double price, CancellationToken cancellationToken)
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
    [Authorize(Policy = AuthorizationPolicies.Admin)]
    public async Task<IActionResult> DeleteProduct([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var request = new ArchiveProductRequest
        {
            ProductId = id
        };

        await _mediator.Send(request, cancellationToken);

        return Ok();
    }


    // 8. Server time
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


    //9. Assign supplier
    [HttpPost("{id}/assign-supplier/{supplierId}")]
    [Authorize(Policy = AuthorizationPolicies.Admin)]
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
    
    // 8. Upload image
    [HttpPost("{id}/image")]
    [Authorize(Policy = AuthorizationPolicies.Admin)]
    public async Task<IActionResult> UploadImage([FromRoute] Guid id, IFormFile? file, CancellationToken cancellationToken)
    {
        if (file == null || file.Length == 0)
            return BadRequest("No file was uploaded.");

        const long maxFileSize = 5 * 1024 * 1024;

        if (file.Length > maxFileSize)
            return BadRequest("Image cannot exceed 5 MB.");

        var allowedContentTypes = new[]
        {
            "image/jpeg",
            "image/png",
        };

        if (!allowedContentTypes.Contains(file.ContentType))
            return BadRequest("Only Jpeg and Png images are allowed.");

        var fileName = Path.GetFileName(file.FileName);

        await using var stream = file.OpenReadStream();

        var request = new AddProductImageRequest
        {
            ProductId = id,
            FileName = fileName,
            ContentType = file.ContentType,
            FileSize = file.Length,
            FileStream = stream
        };

        var result = await _mediator.Send(request, cancellationToken);

        return Ok(result);
    }
    
    // 11. Get product images
    [HttpGet("{id}/images")]
    public async Task<IActionResult> GetProductImages([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var request = new GetProductImagesRequest
        {
            ProductId = id
        };

        var images = await _mediator.Send(request, cancellationToken);

        return Ok(images);
    }
    
    // 12. Download product image
    [HttpGet("images/{imageId}/download")]
    public async Task<IActionResult> DownloadImage([FromRoute] Guid imageId, CancellationToken cancellationToken)
    {
        var request = new DownloadProductImageRequest
        {
            ImageId = imageId
        };

        var result = await _mediator.Send(request, cancellationToken);

        var extension = Path.GetExtension(result.FileName).ToLowerInvariant();

        var contentType = extension switch
        {
            ".png" => "image/png",
            ".jpg" or ".jpeg" => "image/jpeg",
            _ => "application/octet-stream"
        };

        return File(result.FileData, contentType, result.FileName);
    }
    
    // 13. Replace product image
    [HttpPut("images/{imageId}")]
    [Authorize(Policy = AuthorizationPolicies.Admin)]
    public async Task<IActionResult> ReplaceImage([FromRoute] Guid imageId, IFormFile? file, CancellationToken cancellationToken)
    {
        if (file == null || file.Length == 0)
            return BadRequest("No file was uploaded.");

        const long maxFileSize = 5 * 1024 * 1024;

        if (file.Length > maxFileSize)
            return BadRequest("Image cannot exceed 5 MB.");

        var allowedContentTypes = new[]
        {
            "image/jpeg",
            "image/png"
        };

        if (!allowedContentTypes.Contains(file.ContentType))
            return BadRequest("Only JPEG and PNG images are allowed.");

        var fileName = Path.GetFileName(file.FileName);
        await using var stream = file.OpenReadStream();

        var request = new ReplaceProductImageRequest
        {
            ImageId = imageId,
            FileName = fileName,
            ContentType = file.ContentType,
            FileSize = file.Length,
            FileStream = stream
        };

        var result = await _mediator.Send(request, cancellationToken);

        return Ok(result);
    }
    
    // 14. Delete product image
    [HttpDelete("images/{imageId}")]
    [Authorize(Policy = AuthorizationPolicies.Admin)]
    public async Task<IActionResult> DeleteImage([FromRoute] Guid imageId, CancellationToken cancellationToken)
    {
        var request = new DeleteProductImageRequest
        {
            ImageId = imageId
        };

        await _mediator.Send(request, cancellationToken);

        return Ok();
    }
    
}