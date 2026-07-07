using Microsoft.AspNetCore.Mvc;
using warehouse_management.Models;
using warehouse_management.Contracts;


namespace warehouse_management.Controllers;

[ApiController]
[Route("api/suppliers")]

public class SuppliersController : ControllerBase
{
    private readonly SupplierService _supplierService;

    

    public SuppliersController(SupplierService supplierService)
    {
        _supplierService = supplierService;
    }
    
// 1. Get all suppliers
[HttpGet]
public IActionResult GetAllSuppliers()
{
    return Ok(_supplierService.GetAllSuppliers());
 } 
    
// 2. Get supplier by id
    [HttpGet("{id}")]
    public IActionResult GetSupplierById([FromRoute] string id)
    {
        var supplier = _supplierService.GetSupplierById(id);

        if (supplier == null)
            return NotFound("Supplier not found.");

        return Ok(supplier);
    }

// 3. Create supplier
    [HttpPost]
    public IActionResult CreateSupplier([FromBody] CreateSupplierRequest request)
    {
        var supplier = _supplierService.CreateSupplier(request);

        if (supplier == null)
            return BadRequest("Invalid request.");

        return CreatedAtAction(nameof(GetSupplierById), new { id = supplier.Id }, supplier);
    }
    
    // 4. Deactivate supplier
    [HttpDelete("{id}")]
    public IActionResult DeactivateSupplier([FromRoute] string id)
    {
        var result = _supplierService.DeactivateSupplier(id);

        if (!result)
            return NotFound("Supplier not found.");

        return Ok();
        
        
    }
    
    }
    

