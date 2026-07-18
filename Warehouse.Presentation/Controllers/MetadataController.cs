using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Warehouse.Application.Commands.Products.CreateProduct;
using Warehouse.Application.Commands.Suppliers.CreateSupplier;
using Warehouse.Application.Exceptions;
using Warehouse.Application.Metadata;
using Warehouse.Presentation;


namespace Warehouse.Presentation.Controllers;

[ApiController]
[Route("api/metadata")]
public class MetadataController : ControllerBase
{
    private readonly IStringLocalizer<SharedResources> _localizer;

    public MetadataController(IStringLocalizer<SharedResources> localizer)
    {
        _localizer = localizer;
    }
    
    
    // 1. Get DTO validation metadata
    [HttpGet("validation/{dtoName}")]
    public IActionResult GetValidationMetadata([FromRoute] string dtoName)
    {
        if (dtoName.Equals(nameof(CreateProductRequest),StringComparison.OrdinalIgnoreCase))
        {
            return Ok(ValidationMetadataHelper.GetMetadata<CreateProductRequest>());
        }

        if (dtoName.Equals(nameof(CreateSupplierRequest),StringComparison.OrdinalIgnoreCase))
        {
            return Ok(ValidationMetadataHelper.GetMetadata<CreateSupplierRequest>());
        }

        throw new NotFoundException(_localizer["DtoNotFound"].Value);
    }
}