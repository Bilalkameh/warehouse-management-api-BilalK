using MediatR;
using Microsoft.AspNetCore.Mvc;
using Warehouse.Application.Commands.Suppliers.CreateSupplier;
using Warehouse.Application.Commands.Suppliers.DeactivateSupplier;
using Warehouse.Application.Queries.Suppliers.GetSupplierById;
using Warehouse.Application.Queries.Suppliers.GetSuppliers;
using Microsoft.AspNetCore.Authorization;
using Warehouse.Presentation.Authorization;
using Warehouse.Application.Commands.Suppliers.AddSupplierDocument;
using Warehouse.Application.Commands.Suppliers.DeleteSupplierDocument;
using Warehouse.Application.Commands.Suppliers.ReplaceSupplierDocument;
using Warehouse.Application.Queries.Suppliers.DownloadSupplierDocument;
using Warehouse.Application.Queries.Suppliers.GetSupplierDocuments;

namespace Warehouse.Presentation.Controllers;

[ApiController]
[Route("api/suppliers")]
[Authorize(Policy = AuthorizationPolicies.User)]
public class SuppliersController : ControllerBase
{
    private readonly IMediator _mediator;

    public SuppliersController(IMediator mediator)
    {
        _mediator = mediator;
    }


    // 1. Get all suppliers
    [HttpGet]
    public async Task<IActionResult> GetAllSuppliers(CancellationToken cancellationToken)
    {
        var request = new GetSuppliersRequest();
        var suppliers = await _mediator.Send(request, cancellationToken);

        return Ok(suppliers);
    }


    // 2. Get supplier by id
    [HttpGet("{id}")]
    public async Task<IActionResult> GetSupplierById([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var request = new GetSupplierByIdRequest
        {
            SupplierId = id
        };

        var supplier = await _mediator.Send(request,  cancellationToken);

        return Ok(supplier);
    }



    // 3. Create supplier
    [HttpPost]
    [Authorize(Policy = AuthorizationPolicies.Admin)]
    public async Task<IActionResult> CreateSupplier([FromBody] CreateSupplierRequest request, CancellationToken cancellationToken)
    {
        var supplier = await _mediator.Send(request, cancellationToken);

        return CreatedAtAction(
            nameof(GetSupplierById),
            new { id = supplier.Id },
            supplier);
    }
    
    // 4. Deactivate supplier
    [HttpDelete("{id}")]
    [Authorize(Policy = AuthorizationPolicies.Admin)]

    public async Task<IActionResult> DeactivateSupplier([FromRoute] Guid id,  CancellationToken cancellationToken)
    {
        var request = new DeactivateSupplierRequest
        {
            SupplierId = id
        };

        await _mediator.Send(request,  cancellationToken);

        return Ok();
    }
    
    // 5. Upload Document
    [HttpPost("{id}/documents")]
    [Authorize(Policy = AuthorizationPolicies.Admin)]
    public async Task<IActionResult> UploadDocument([FromRoute] Guid id, IFormFile? file, CancellationToken cancellationToken)
    {
        if (file == null || file.Length == 0)
            return BadRequest("No file was uploaded.");

        const long maxFileSize = 10 * 1024 * 1024;

        if (file.Length > maxFileSize)
            return BadRequest("Document cannot exceed 10 MB.");

        if (file.ContentType != "application/pdf")
            return BadRequest("Only PDF documents are allowed.");

        var fileName = Path.GetFileName(file.FileName);

        await using var stream = file.OpenReadStream();

        var request = new AddSupplierDocumentRequest
        {
            SupplierId = id,
            FileName = fileName,
            ContentType = file.ContentType,
            FileSize = file.Length,
            FileStream = stream
        };

        var result = await _mediator.Send(request, cancellationToken);

        return Ok(result);
    }
    
    //6. Get supplier documents
    [HttpGet("{id}/documents")]
    public async Task<IActionResult> GetSupplierDocuments([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var request = new GetSupplierDocumentsRequest
        {
            SupplierId = id
        };

        var documents = await _mediator.Send(request, cancellationToken);

        return Ok(documents);
    }
    
    // 7. Download document
    [HttpGet("documents/{documentId}/download")]
    public async Task<IActionResult> DownloadDocument([FromRoute] Guid documentId, CancellationToken cancellationToken)
    {
        var request = new DownloadSupplierDocumentRequest
        {
            DocumentId = documentId
        };

        var result = await _mediator.Send(request, cancellationToken);

        return File(result.FileData, "application/pdf", result.FileName);
    }
    
    //8. Replace document
    [HttpPut("documents/{documentId}")]
    [Authorize(Policy = AuthorizationPolicies.Admin)]
    public async Task<IActionResult> ReplaceDocument([FromRoute] Guid documentId, IFormFile? file, CancellationToken cancellationToken)
    {
        if (file == null || file.Length == 0)
            return BadRequest("No file was uploaded.");

        const long maxFileSize = 10 * 1024 * 1024;

        if (file.Length > maxFileSize)
            return BadRequest("Document cannot exceed 10 MB.");

        if (file.ContentType != "application/pdf")
            return BadRequest("Only PDF documents are allowed.");

        var fileName = Path.GetFileName(file.FileName);

        await using var stream = file.OpenReadStream();

        var request = new ReplaceSupplierDocumentRequest
        {
            DocumentId = documentId,
            FileName = fileName,
            ContentType = file.ContentType,
            FileSize = file.Length,
            FileStream = stream
        };

        var result = await _mediator.Send(request, cancellationToken);

        return Ok(result);
    }
    
    // 9. Delete document
    [HttpDelete("documents/{documentId}")]
    [Authorize(Policy = AuthorizationPolicies.Admin)]
    public async Task<IActionResult> DeleteDocument([FromRoute] Guid documentId, CancellationToken cancellationToken)
    {
        var request = new DeleteSupplierDocumentRequest
        {
            DocumentId = documentId
        };

        await _mediator.Send(request, cancellationToken);

        return Ok();
    }
    
    
}