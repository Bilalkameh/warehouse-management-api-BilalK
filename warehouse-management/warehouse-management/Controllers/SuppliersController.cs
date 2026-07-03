using Microsoft.AspNetCore.Mvc;
using warehouse_management.Models;
using warehouse_management.Contracts;


namespace warehouse_management.Controllers;

[ApiController]
[Route("api/suppliers")]

public class SuppliersController : ControllerBase
{
    private  readonly FakeSupplierStore _store;

    public SuppliersController()
    {
        _store = new FakeSupplierStore();
    }
    
// 1. Get all suppliers
[HttpGet]
public IActionResult GetAllSuppliers()
{
        var suppliers = _store.GetAllSuppliers()
            .Where(s => s.IsActive)
            .ToList();
        return Ok(suppliers);
 } 
    
// 2. Get supplier by id
    [HttpGet("{id}")]
    public IActionResult GetSupplierById([FromRoute] string id)
    {
        if (string.IsNullOrWhiteSpace(id))
            return BadRequest("Id required.");
        
        
        var supplier = _store.GetSupplierById(id);
        
        
        if (supplier == null || !supplier.IsActive)
            return NotFound("Supplier not found.");
        
        return Ok(supplier);
    }

// 3. Create supplier
    [HttpPost]
    public IActionResult CreateSupplier([FromBody] CreateSupplierRequest request)
    {
        if (request == null)
            return BadRequest("Invalid request.");
        
        if (string.IsNullOrWhiteSpace(request.Name))
            return BadRequest("Supplier name required.");
        
        if (string.IsNullOrWhiteSpace(request.Country))
            return BadRequest("Country required.");
        
        if (string.IsNullOrWhiteSpace(request.ContactEmail))
            return BadRequest("Contact email required.");
        
        if (string.IsNullOrWhiteSpace(request.PhoneNumber))
            return BadRequest("Phone number required.");
        
        var newSupplier = new Supplier(
            Guid.NewGuid().ToString(),
            request.Name,
            request.Country,
            request.ContactEmail,
            request.PhoneNumber
        );
        
        _store.AddSupplier(newSupplier);

        
        return CreatedAtAction(nameof(GetSupplierById), new { id = newSupplier.Id }, newSupplier);
    }
    
    // 4. Deactivate supplier
    [HttpDelete("{id}")]
    public IActionResult DeactivateSupplier([FromRoute] string id)
    {
        if (string.IsNullOrWhiteSpace(id))
            return BadRequest("Id required.");
        
        var supplier = _store.GetSupplierById(id);
        
        if (supplier == null || !supplier.IsActive)
            return NotFound("Supplier not found.");

        
        var result = _store.Deactivate(id);
        
        if (!result)
            return NotFound("Supplier not found.");
        
        return Ok();
    }
    }
    

