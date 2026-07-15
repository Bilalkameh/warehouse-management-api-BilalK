using MediatR;
using Microsoft.AspNetCore.Mvc;
using Warehouse.Application.Commands.Suppliers.CreateSupplier;
using Warehouse.Application.Commands.Suppliers.DeactivateSupplier;
using Warehouse.Application.Queries.Suppliers.GetSupplierById;
using Warehouse.Application.Queries.Suppliers.GetSuppliers;

namespace Warehouse.Presentation.Controllers;

[ApiController]
[Route("api/suppliers")]
public class SuppliersController : ControllerBase
{
    private readonly IMediator _mediator;

    public SuppliersController(IMediator mediator)
    {
        _mediator = mediator;
    }


    // 1. Get all suppliers
    [HttpGet]
    public async Task<IActionResult> GetAllSuppliers()
    {
        var request = new GetSuppliersRequest();
        var suppliers = await _mediator.Send(request);

        return Ok(suppliers);
    }


    // 2. Get supplier by id
    [HttpGet("{id}")]
    public async Task<IActionResult> GetSupplierById([FromRoute] Guid id)
    {
        var request = new GetSupplierByIdRequest
        {
            SupplierId = id
        };

        var supplier = await _mediator.Send(request);

        return Ok(supplier);
    }



    // 3. Create supplier
    [HttpPost]
    public async Task<IActionResult> CreateSupplier([FromBody] CreateSupplierRequest request)
    {
        var supplier = await _mediator.Send(request);

        return CreatedAtAction(
            nameof(GetSupplierById),
            new { id = supplier.Id },
            supplier);
    }
    
    // 4. Deactivate supplier
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeactivateSupplier([FromRoute] Guid id)
    {
        var request = new DeactivateSupplierRequest
        {
            SupplierId = id
        };

        await _mediator.Send(request);

        return Ok();
    }
}