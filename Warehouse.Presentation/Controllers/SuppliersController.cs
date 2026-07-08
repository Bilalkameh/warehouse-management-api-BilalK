using Microsoft.AspNetCore.Mvc;
using Warehouse.Application.Commands.Suppliers;
using Warehouse.Application.Handlers.Suppliers;
using Warehouse.Application.Queries.Suppliers;
using Warehouse.Domain.Entities;

namespace Warehouse.Presentation.Controllers;

[ApiController]
[Route("api/suppliers")]
public class SuppliersController : ControllerBase
{
    private readonly GetSuppliersHandler _getSuppliersHandler;
    private readonly GetSupplierByIdHandler _getSupplierByIdHandler;
    private readonly CreateSupplierHandler _createSupplierHandler;
    private readonly DeactivateSupplierHandler _deactivateSupplierHandler;


    public SuppliersController(
        GetSuppliersHandler getSuppliersHandler,
        GetSupplierByIdHandler getSupplierByIdHandler,
        CreateSupplierHandler createSupplierHandler,
        DeactivateSupplierHandler deactivateSupplierHandler)
    {
        _getSuppliersHandler = getSuppliersHandler;
        _getSupplierByIdHandler = getSupplierByIdHandler;
        _createSupplierHandler = createSupplierHandler;
        _deactivateSupplierHandler = deactivateSupplierHandler;
    }


    // 1. Get all suppliers
    [HttpGet]
    public IActionResult GetAllSuppliers()
    {
        var query = new GetSuppliersQuery();

        var suppliers = _getSuppliersHandler.Handle(query);

        return Ok(suppliers);
    }


    // 2. Get supplier by id
    [HttpGet("{id}")]
    public IActionResult GetSupplierById([FromRoute] Guid id)
    {
        var query = new GetSupplierByIdQuery
        {
            SupplierId = id
        };

        var supplier = _getSupplierByIdHandler.Handle(query);

        if (supplier == null)
            return NotFound("Supplier not found.");

        return Ok(supplier);
    }


    // 3. Create supplier
    [HttpPost]
    public IActionResult CreateSupplier([FromBody] CreateSupplierCommand command)
    {
        var supplier = _createSupplierHandler.Handle(command);

        return CreatedAtAction(
            nameof(GetSupplierById),
            new { id = supplier.Id },
            supplier);
    }
    
    // 4. Deactivate supplier
    [HttpDelete("{id}")]
    public IActionResult DeactivateSupplier([FromRoute] Guid id)
    {
        var command = new DeactivateSupplierCommand
        {
            SupplierId = id
        };
        var result = _deactivateSupplierHandler.Handle(command);

        if (!result)
            return NotFound("Supplier not found.");

        return Ok();
    }
}