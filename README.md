# warehouse-management-api-BilalK

(Session-02) Building REST APIs & API Documentation

In this session, I developed a RESTful Web API using ASP.NET Core for a simple Warehouse Management System.  
The API manages products and suppliers using in-memory storage. This can be found in the FakeSupplierStore and FakeWarehouseStore classes in static lists.

# Features

Product Management
The implemented ProductsController.cs controller's endpoints allow to:
1- Retrieve all products
2- Retrieve any product by ID
3- Search products by name and/or supplier
4- Create new products 
5- Update product quantity
6- Update product price
7- Upload product images with validation 
8- Delete Products (soft deletion/Archiving)
9- Return current date formatted according to language with 3 languages options : en-Us , fr-FR, ar-LB. The user has the choice. (This is more of a utility feature and does not actually modify or affect the products).


Supplier Management:
Similarly The implemented SuppliersController.cs controller's endpoints allow to:
1- Retrieve all suppliers
2- Retrieve supplier by ID
3- Create supplier
4- Deactivate supplier (soft delete. Similar to the soft deletin of the products sets the supplier to inactive)


Project Structure:
The implementation can be found inside the warehouse-management folder:

`Models/` -> Domain entities (Product, Supplier, ProductImage)
`Contracts/` -> Request DTOs for API inputs
`Controllers/` -> Controllers with API endpoints (Both Products and Suppliers Controllers)
`FakeWarehouseStore` -> In-memory product storage
`FakeSupplierStore` -> In-memory supplier storage


Refactoring (Controller-Service Pattern):
project was refactored to follow a Controller-Service separation pattern as required by the lab instructions and reminded bi Ms Andrea
At first the controllers contained both HTTP handling logic and business logic.
This was improved by adding a Service layer:
1- ProductService
2- SupplierService



Endpoints:
- Products:
 GET     /api/products -> Get all products  
 GET     /api/products/{id} -> Get product by ID
 GET     /api/products/search -> Search products
 GET     /api/products/server-time -> Get server time
 POST    /api/products -> Create product
 POST    /api/products/{id}/quantity -> Update quantity
 POST    /api/products/{id}/price -> Update price       (Also could have been PUT)
 POST    /api/products/{id}/image -> Upload product image   (Also could have been PUT)
 DELETE  /api/products/{id} -> Soft delete product
 POST    //api/products/{id}/assign-supplier/{supplierId} -> Assign supplier to product

 - Suppliers:
 GET     /api/suppliers -> Get all suppliers
 GET     /api/suppliers/{id} -> Get supplier by ID
 POST    /api/suppliers -> Create supplier
 DELETE  /api/suppliers/{id} -> Deactivate supplier

----------------------------------------------------------------------------------------

(Session-03) Refactoring Warehouse API using DDD Architecture

In this session, the existing Warehouse Management API was refactored from a Controller-Service architecture 
into an architecture following Domain-Driven Design (DDD) principles.

The goal of this refactor was to separate business rules, application logic, data access, 
and HTTP handling while keeping the existing API behavior unchanged.

# Features
### Architecture Layers
The project was reorganized into four main layers:


### Warehouse.Domain
Responsible for core business logic and domain models.

Implemented entities:
- Product
- Supplier
- ProductImage
- StockMovement
- WarehouseItem

Note:
`StockMovement` and `WarehouseItem` were added as part of the required domain structure. 
However, they are currently incomplete because the existing API requirements from previous sessions did not include stock movement tracking or warehouse item management functionality.
Also I am unsure exactly what to place inside them
They are prepared as future domain entities and can be extended when those modules are implemented.

Implemented business rules:
- Product name is required
- SKU is required
- Price must be greater than zero
- Quantity cannot be negative
- Archived products cannot be updated
- Inactive suppliers cannot be assigned to products

Repository contracts are also in this layer:
- IProductRepository
- ISupplierRepository
- IProductImageRepository


### Warehouse.Application
Contains application use cases and separates commands from queries using a CQRS organization.

Implemented commands:
- CreateProduct
- UpdateProductQuantity
- UpdateProductPrice
- ArchiveProduct
- AssignSupplierToProduct
- CreateSupplier
- DeactivateSupplier
- AddProductImage

Implemented queries:
- GetProductById
- ListProducts
- SearchProducts
- GetSupplierById
- ListSuppliers

Also Added all the appropriate handlers split into separate Products and Suppliers folders for a cleaner organisation.


### Warehouse.Infrastructure
Responsible for data access implementation.

Implemented repositories:
- FakeProductRepository
- FakeSupplierRepository
- FakeProductImageRepository

These are implementation of the repository interfacer from the Domain layer.
They continue using in-memory storage while hiding the storage details from the Application and Domain layers.

Note:
- The lab mentions MediatR as a harder requirement for implementing CQRS. 
- MediatR was not used in this refactor because the current command/query is already achieved in the application layer. 
- It will certainly be added in future 

### Warehouse.Presentation
Contains the API layer.

Controllers were refactored to become thin controllers:
- Receive HTTP requests
- Create commands/queries
- Call application handlers
- Return HTTP responses

All the endpoints from session-02 still exist and are working and testable.
There is zero business logic or storage access in the controllers.


# Design Patterns and Concepts Used
1- Domain-Driven Design (DDD)
2- Repository Pattern
3- Dependency Injection
4- CQRS-style Organization


# Run instructions
dotnet restore
dotnet build
dotnet run --project Warehouse.Presentation

Then open: http://localhost:5035/swagger


# Screenshots
Swagger screenshots showing the refactored API behavior will be included in the Pull Request.

