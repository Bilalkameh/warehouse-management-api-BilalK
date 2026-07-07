using Microsoft.AspNetCore.Mvc;
using warehouse_management.Models;
using warehouse_management.Contracts;

namespace warehouse_management.Controllers;

[ApiController]
[Route("api/products")]
public class ProductsController : ControllerBase
{       
        
        private readonly FakeWarehouseStore _store;
        // Add dependency in order to implement Task 2 (Product-Supplier Link)
        private readonly FakeSupplierStore _supplierStore;
        
        // Add ProductService field in order to refactor
        private readonly ProductService _productService;

        public ProductsController(FakeWarehouseStore store, FakeSupplierStore supplierStore, ProductService productService)
        {
                _store = store;
                _supplierStore = supplierStore;
                _productService = productService;
        }
        
        // 1. Get all products 
        [HttpGet()]
        public IActionResult GetAllProducts([FromQuery] bool onlyAvailable = false)
        {
                var products = _productService.GetAllProducts(onlyAvailable);
                return Ok(products);
        }
        
        // 2. Get product by id 
        [HttpGet("{id}")]
        public IActionResult GetProductById([FromRoute] string id)
        {       
                
                var product = _productService.GetProductById(id);

                if (product == null)
                        return NotFound("Product not found");

                return Ok(product);
        }
        
        // 3. Search products
        [HttpGet("search")]
        public IActionResult SearchProducts([FromQuery] string? name,[FromQuery] string? supplier)
        {
                var products = _productService.SearchProducts(name, supplier);

                if (products == null)
                        return BadRequest("a product name or a Supplier must be provided");

                return Ok(products);
        }
        
        // 4. Create product
        [HttpPost()]
        public IActionResult CreateProduct([FromBody] CreateProductRequest request)
        {
                var product = _productService.CreateProduct(request);

                if (product == null)
                        return BadRequest("SKU already exists");

                return CreatedAtAction(nameof(GetProductById), new { id = product.Id }, product);
        }
        
        // 5. Update Quantity

        [HttpPost("{id}/quantity")]
        public IActionResult UpdateQuantity([FromRoute] string id, [FromBody] UpdateProductQuantityRequest request)
        {
                var result = _productService.UpdateQuantity(id, request.QuantityInStock);

                if (!result)
                        return BadRequest("Invalid product or quantity");

                return Ok();
        }
        
        //6. Update Price 

        [HttpPost("{id}/price")]
        public IActionResult UpdatePrice([FromRoute] string id, [FromBody] UpdateProductPriceRequest request)
        {
                var result = _productService.UpdatePrice(id, request.Price);

                if (!result)
                        return BadRequest("Invalid product or price");

                return Ok();
        }
        
        
        // 7. Upload image 
        // This one was difficult I had to use AI also caused many bugs that I tried to fix using AI and finally it works
        
        [HttpPost("{id}/image")]
        public async Task<IActionResult> UploadImage([FromRoute] string id, IFormFile? file)
        {
                if (file == null || file.Length == 0)
                        return BadRequest("No file was uploaded.");

                var result = _productService.AddProductImage(id, file);

                if (result == null)
                        return BadRequest("Invalid product or file.");

                return Ok(result);
        }
        
        // 8. Delete product : This is the soft delete by setting set IsArchived = true 

        [HttpDelete("{id}")]
        public IActionResult DeleteProduct([FromRoute] string id)
        {
                var result = _productService.DeleteProduct(id);

                if (!result)
                        return NotFound("Product not found");

                return Ok();
        }
        
        // 9.  Get warehouse server time 
        // Here we dont actually have to refactor anything because this endpoint is stateless.
        // Also does not require any interactionz with the in-memory data

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
                
                var formattedTime = now.ToString("F", culture);
                return Ok(new { serverTime = formattedTime });


        }
        
        // 10. Assign Supplier to Product
        [HttpPost("{id}/assign-supplier/{supplierId}")]
        public IActionResult AssignSupplier([FromRoute] string id, [FromRoute] string supplierId)
        {
                var product = _productService.AssignSupplier(id, supplierId, _supplierStore);

                if (product == null)
                        return BadRequest("Invalid product or supplier, or already assigned");

                return Ok(product);
        }
}