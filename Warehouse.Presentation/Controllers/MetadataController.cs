using Microsoft.AspNetCore.Mvc;
using Warehouse.Application.Commands.Products.CreateProduct;
using Warehouse.Application.Commands.Suppliers.CreateSupplier;
using Warehouse.Application.Exceptions;
using Warehouse.Application.Metadata;
using Warehouse.Presentation.Resources;


namespace Warehouse.Presentation.Controllers;

[ApiController]
[Route("api/metadata")]
public class MetadataController : ControllerBase
{

    public MetadataController()
    {
        
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

        throw new NotFoundException(SharedResources.DtoNotFound);
    }
}