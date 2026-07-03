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
                products = products.Where(p => !p.IsArchived).ToList();
                
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
                
                if (string.IsNullOrWhiteSpace(id))
                        return BadRequest("Invalid id");
                
                
                var  product = _store.GetById(id);
                
                if (product == null || product.IsArchived)
                        return NotFound("Product not found ");
                
                return Ok(product);
        }
        
        // 3. Search products
        [HttpGet("search")]
        public IActionResult SearchProducts([FromQuery] string? name,[FromQuery] string? supplier)
        {
                if ( string.IsNullOrWhiteSpace(name) && string.IsNullOrWhiteSpace(supplier))
                        return BadRequest("a product name or a Supplier must be provided");
                
                var products = _store.Search(name, supplier)
                                .Where(p => !p.IsArchived)
                                .ToList();
                
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
        
        // 5. Update Quantity

        [HttpPost("{id}/quantity")]
        public IActionResult UpdateQuantity([FromRoute] string id, [FromBody] UpdateProductQuantityRequest request)
        {
                if (request.QuantityInStock < 0)
                        return BadRequest("Cannot update negative quantity");
                
                var product = _store.GetById(id);
                
                if (product == null || product.IsArchived)
                        return NotFound("Product not found");
                

                var result = _store.UpdateQuantity(id, request.QuantityInStock);
                
                if (!result)
                        return NotFound("Product not found");
                
                Console.WriteLine("Quantity updated. New Quantity is: " + request.QuantityInStock);
                return Ok();
        }
        
        //6. Update Price 

        [HttpPost("{id}/price")]
        public IActionResult UpdatePrice([FromRoute] string id, [FromBody] UpdateProductPriceRequest request)
        {
                if (request.Price <= 0)
                        return BadRequest("Cannot update negative price");
                
                var product = _store.GetById(id);
                
                if  (product == null || product.IsArchived)
                        return NotFound("Product not found");

                var oldPrice = product.Price;
                
                var result = _store.UpdatePrice(id, request.Price);
                
                if (!result)
                        return NotFound("Product not found");

                Console.WriteLine($"Price updated from : {oldPrice} to {request.Price}");
                return Ok();
        }
        
        // 7. Upload image 
        // This one was difficult I had to use AI
        [HttpPost("{id}/image")]
        public IActionResult UpdateImage([FromRoute] string id, [FromForm] IFormFile file)
        {
                var product = _store.GetById(id);
                
                if (product == null || product.IsArchived)
                        return NotFound("Product not found");
                
                if (file == null || file.Length == 0)
                        return BadRequest("File is empty");
                
                var possibleExtensions = new[] { ".jpg", ".png" };

                var extension = Path.GetExtension(file.FileName).ToLower();

                if (!possibleExtensions.Contains(extension))
                        return BadRequest("Invalid file extension");
                
                if (file.Length > 2*1024*1024)
                        return BadRequest("File too large");
                
                var folder = Path.Combine("wwwroot", "uploads");
                
                if (!Directory.Exists(folder))
                        Directory.CreateDirectory(folder);
                
                var name = file.FileName;   
                var path = Path.Combine(folder, name);

                using (var stream = new FileStream(path, FileMode.Create))
                {
                        file.CopyTo(stream);
                }
                
                return Ok(new {name, path});
        }
        
        // 8. Delete product 

        [HttpDelete("{id}")]
        public IActionResult DeleteProduct([FromRoute] string id)
        {
                var product = _store.GetById(id);
                if  (product == null || product.IsArchived)
                        return NotFound("Product not found");
                
                product.IsArchived = true;
                product.LastUpdatedAt =  DateTime.UtcNow;

                return Ok();
        }
        
        // 9.  Get warehouse server time 
        // I didnt really understand this one so i also had AI assitance 

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



}