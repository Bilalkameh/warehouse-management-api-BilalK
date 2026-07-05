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
        

        public ProductsController(FakeWarehouseStore store, FakeSupplierStore supplierStore)
        {
                _store = store;
                _supplierStore = supplierStore;
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
        // This one was difficult I had to use AI also caused many bugs that I tried to fix using AI and finally it works
        
        [HttpPost("{id}/image")]
        public async Task<IActionResult> UploadImage([FromRoute] string id, IFormFile? file)
        {
                var product = _store.GetById(id);

                if (product is null)
                {
                        return NotFound($"Product with id '{id}' was not found.");
                }

                if (file is null || file.Length == 0)
                {
                        return BadRequest("No file was uploaded.");
                }

                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
                var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
                if (!allowedExtensions.Contains(extension))
                {
                        return BadRequest("Only .jpg, .jpeg, and .png files are allowed.");
                }

                const long maxSizeBytes = 2 * 1024 * 1024; // 2 MB
                if (file.Length > maxSizeBytes)
                {
                        return BadRequest("File size must not exceed 2 MB.");
                }

                var uploadsFolder = Path.Combine(
                        Directory.GetCurrentDirectory(), "wwwroot", "uploads");
                Directory.CreateDirectory(uploadsFolder);

                var fileName = $"{Guid.NewGuid()}{extension}";
                var filePath = Path.Combine(uploadsFolder, fileName);

                await using (var stream = new FileStream(filePath, FileMode.Create))
                {
                        await file.CopyToAsync(stream);
                }

                var productImage = new ProductImage
                {
                        ProductId = product.Id,
                        FileName = fileName,
                        FilePath = $"/uploads/{fileName}"
                };
                product.LastUpdatedAt = DateTime.UtcNow;

                return Ok(productImage);
        }
        
        // 8. Delete product : This is the soft delete by setting set IsArchived = true 

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
                if (string.IsNullOrWhiteSpace(id) || string.IsNullOrWhiteSpace(supplierId))
                        return BadRequest("Product id and Supplier id are required");
                
                var product = _store.GetById(id);
                if (product == null || product.IsArchived)
                        return NotFound("Product not found.");
                
                var supplier = _supplierStore.GetSupplierById(supplierId);
                if (supplier == null || !supplier.IsActive)
                        return NotFound("Supplier not found.");
                
                // This is added to prevent redundant reassignment
                if (product.SupplierName == supplier.Name)
                        return BadRequest("Supplier already assigned to this product");
                
                product.SupplierName = supplier.Name;
                product.LastUpdatedAt = DateTime.UtcNow;
                
                return Ok(product);

        }
        




}