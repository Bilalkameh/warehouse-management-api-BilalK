using Microsoft.AspNetCore.Mvc;
using Warehouse.Application.Commands.Products.CreateProduct;
using Warehouse.Application.Commands.Suppliers.CreateSupplier;
using Warehouse.Application.Exceptions;
using Warehouse.Application.Metadata;

namespace Warehouse.Presentation.Controllers;

[ApiController]
[Route("api/metadata")]
public class MetadataController : ControllerBase
{
    // 1. Get DTO validation metadata
    [HttpGet("validation/{dtoName}")]
    public IActionResult GetValidationMetadata(
        [FromRoute] string dtoName)
    {
        if (dtoName.Equals(nameof(CreateProductRequest), StringComparison.OrdinalIgnoreCase))
        {
            var metadata = ValidationMetadataHelper.GetMetadata<CreateProductRequest>();

            return Ok(metadata);
        }

        if (dtoName.Equals(nameof(CreateSupplierRequest), StringComparison.OrdinalIgnoreCase))
        {
            var metadata = ValidationMetadataHelper.GetMetadata<CreateSupplierRequest>();

            return Ok(metadata);
        }

        throw new NotFoundException("DTO was not found.");
    }
}