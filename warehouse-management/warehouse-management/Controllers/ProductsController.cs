using System.Runtime.InteropServices.JavaScript;
using Microsoft.AspNetCore.Mvc;
using warehouse_management.Models;
using warehouse_management.Contracts;

namespace warehouse_management.Controllers;

[ApiController]
[Route("api/products")]
public class ProductsController : ControllerBase
{
        private readonly FakeWarehouseStore _store;

        public ProductsController()
        {
                _store = new FakeWarehouseStore();
        }
        
        // 1. Get all products 
        [HttpGet()]
        public IActionResult GetAllProducts([FromQuery] bool onlyAvailable = false)
        {
                var products = _store.GetAll();
                if (onlyAvailable)
                {
                        products = products.Where(p => p.QuantityInStock > 0).ToList();
                }

                products = products.OrderByDescending(p => p.CreatedAt)
                        .ToList();

                return Ok(products);
        }
        
        // 2. Get product by id 
        [HttpGet("{id}")]
        public IActionResult GetProductById([FromRoute] string id)
        {       
                
                
                if (!Guid.TryParse(id, out Guid guid))
                        return BadRequest("Not a valid GUID");
                
                
                var  product = _store.GetById(id);
                
                if (product == null)
                {
                        return NotFound("Product not found ");
                }
                return Ok(product);
        }
        
        // 3. Search products
        [HttpGet("search")]
        public IActionResult SearchProducts([FromQuery] string? name,[FromQuery] string? supplier)
        {
                if ( string.IsNullOrWhiteSpace(name) && string.IsNullOrWhiteSpace(supplier))
                        return BadRequest("a product name or a Supplier must be provided");
                
                var products = _store.Search(name, supplier);
                return Ok(products);
        }
        
        // 4. Create product
        [HttpPost()]
        public IActionResult CreateProduct([FromBody] CreateProductRequest request)
        {
                foreach (var product in _store.GetAll())
                { 
                        //There is actually no point in this check because I never assigned SKU to the existing products
                        //Only way this may trigger is if the user tries to add two products with the same SKU that he assigns
                        if (string.Equals(product.SKU, request.SKU, StringComparison.OrdinalIgnoreCase))
                        {
                                return BadRequest("SKU already exists");
                        }
                }
                
                var newProduct = new Product(Guid.NewGuid().ToString(), request.Name, request.Description, 
                        request.Price, request.QuantityInStock, request.SupplierName, request.ExpiryDate);
                
                newProduct.SKU = request.SKU;
                
                _store.Add(newProduct);
                
                return CreatedAtAction(nameof(GetProductById), new { id = newProduct.Id }, newProduct);
        }
        
}