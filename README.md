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

Models/ -> Domain entities (Product, Supplier, ProductImage)
Contracts/ -> Request DTOs for API inputs
Controllers/ -> Controllers with API endpoints (Both Products and Suppliers Controllers)
FakeWarehouseStore -> In-memory product storage
FakeSupplierStore -> In-memory supplier storage


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
StockMovement and WarehouseItem were added as part of the required domain structure. 
However, they are currently incomplete because the existing API requirements from previous sessions did not include stock movement tracking or warehouse item management functionality.
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
Contains application use cases and separates commands from queries using CQRS with MediatR.

The Application layer is organized with Commands and Queries as the top-level folders.
Inside each one, use cases are grouped by entity/functionality and then by use case.

Example structure:
- Commands/Products/CreateProduct
  - CreateProductRequest.cs
  - CreateProductResponse.cs
  - CreateProductHandler.cs

Implemented product commands:
- CreateProduct
- UpdateProductQuantity
- UpdateProductPrice
- ArchiveProduct
- AssignSupplierToProduct
- AddProductImage

Implemented supplier commands:
- CreateSupplier
- DeactivateSupplier

Implemented product queries:
- GetProductById
- ListProducts
- SearchProducts

Implemented supplier queries:
- GetSupplierById
- ListSuppliers

Each use case contains:
- Request
- Response
- Handler

MediatR is used to send requests from the controllers to the correct handlers.


### Warehouse.Infrastructure
Responsible for data access implementation.

Implemented repositories:
- FakeProductRepository
- FakeSupplierRepository
- FakeProductImageRepository

These are implementations of the repository interfaces from the Domain layer.
They continue using in-memory storage while hiding the storage details from the Application and Domain layers.


### Warehouse.Presentation
Contains the API layer.

Controllers were refactored to become thin controllers:
- Receive HTTP requests
- Create requests
- Send requests using MediatR
- Return HTTP responses

All the endpoints from session-02 still exist and are working and testable.
There is no business logic or storage access in the controllers.


# Design Patterns and Concepts Used
1- Domain-Driven Design (DDD)
2- Repository Pattern
3- Dependency Injection
4- CQRS with MediatR


# Run instructions
dotnet restore
dotnet build
dotnet run --project Warehouse.Presentation

Then open localhost:5035


# Screenshots
Swagger screenshots showing the refactored API behavior will be included in the Pull Request.

----------------------------------------------------------------------------------------

(Session-04) Database Implementation using EF Core Code First

In this session, I replaced the in-memory repositories from session 03 with a real PostgreSQL database using Entity Framework Core Code First.

The Domain and Application layers still depend on repository interfaces, while the Infrastructure layer now implements those interfaces using WarehouseDbContext. Because of this, the controllers and MediatR use cases stayed almost the same even though the storage changed from static lists to a database.

# Features

### PostgreSQL and EF Core
- Added WarehouseDbContext inside the Infrastructure layer.
- Connected the API to PostgreSQL running in Docker.
- Used EF Core migrations to create the WarehouseDb database in datagrip.
- Added database tables and relationships for Products, Suppliers, and ProductImages.

### Real Repository Implementations
The fake repositories were replaced with EF Core repositories:
- ProductRepository
- SupplierRepository
- ProductImageRepository

### AutoMapper and ViewModels
AutoMapper was added to convert database entities into:
- ProductViewModel
- SupplierViewModel

I used ViewModels so the API does not return EF Core entities and navigation properties directly. 
This also avoids returning database details that the client does not need.

# Run instructions

Make sure Docker Desktop is running, then start the PostgreSQL container:


docker start postgresdb
dotnet restore
dotnet ef database update --project Warehouse.Infrastructure --startup-project Warehouse.Presentation
dotnet run --project Warehouse.Presentation

Then open:

http://localhost:5035/swagger

# API endpoints

The main change in this session was how the data is stored, so most route names stayed the same. These endpoints now read and write data from PostgreSQL:

Products:
- GET /api/products -> Get all products
- GET /api/products/{id} -> Get a product by ID
- GET /api/products/search -> Search products by name or supplier
- POST /api/products -> Add a product
- POST /api/products/{id}/quantity -> Update product quantity
- POST /api/products/{id}/price -> Update product price
- POST /api/products/{id}/image -> Add a product image
- POST /api/products/{id}/assign-supplier/{supplierId} -> Assign a supplier
- DELETE /api/products/{id} -> Archive a product

Suppliers:
- GET /api/suppliers -> Get all suppliers
- GET /api/suppliers/{id} -> Get a supplier by ID
- POST /api/suppliers -> Add a supplier
- DELETE /api/suppliers/{id} -> Deactivate a supplier

----------------------------------------------------------------------------------------


(Session-05) Advanced .NET Core

In this session, I focused on making the API more consistent.
Validation, errors, logging, and request information are now handled in common places instead of repeating the same code in every controller or handler.

# Features

### Consistent Errors and Validation
Added one shared ApiErrorResponse containing 3 attributes:
- Error code
- Safe message
- Trace ID

Custom exceptions that are used for cases that the application expects. They simply inherit from the normal Exception class but they use more clear names:
- NotFoundException when a product, supplier, or DTO cannot be found
- BusinessRuleException when a domain rule is broken

Simple property rules now use Data Annotations, for example required names, string lengths, and future expiry dates and so on
Also used FluentValidation for the stock adjustment

Middleware works with the general HTTP context. It does not depend on a specific controller or request context.
For example, CorrelationIdMiddleware adds a correlation ID to every request, it doesnt depend on the context of the request, no matter the request same operation.

Filters work inside MVC and have more information about the context of the action or request. For example, ModelValidationFilter can read ModelState which is 
dependent on the specific request context.  

New middleware:
- CorrelationIdMiddleware -> Reads or creates X-Correlation-ID and also uses it as the trace ID
- ExceptionHandlingMiddleware -> Logs the real exception and returns a safe API response
- RequestTimingMiddleware -> Measures the request and adds X-Response-Time

New filters:
- ModelValidationFilter -> Returns the shared validation response when ModelState is invalid
- ActionLoggingFilter -> Logs which controller action is executing and when it finishes

### Async and Cancellation Tokens
All endpoints are now async and accept a CancellationToken and pass it through MediatR, handlers, repositories, and EF Core queries. 
This is very important because now DB operations dont cause long waiting time and make the application slower

The inventory dashboard loads three independent values:
- Total products
- Available products
- Active suppliers

The three queries are started together and awaited using Task.WhenAll. 
Because there are 3 different await's all have to complete for the operation to happen which is exactly what we want.



### Stock Adjustments
The previously empty StockMovement entity is now implemented and stored in the database.

When stock is adjusted, the handler:
1. Validates the request
2. Loads the product
3. Calculates and applies the new quantity
4. Creates a StockMovement record

Had to make anothe migration to add the table and update the db

# Run instructions

docker start postgresdb
dotnet ef database update --project Warehouse.Infrastructure --startup-project Warehouse.Presentation
dotnet run --project Warehouse.Presentation
Then open localhost:5035


# New endpoints
- POST /api/stock-adjustments -> Increase or reduce a product's quantity and save the movement
- GET /api/inventory/dashboard -> Return total products, available products, and active suppliers


----------------------------------------------------------------------------------------

(Session-07) Firebase Authentication, Authorization and MinIO Object Storage

In this session, I added authentication and authorization to the Warehouse Management API using Firebase, and I introduced MinIO for storing warehouse files.

# Firebase Authentication

A Firebase project was created with Email/Password authentication enabled. I created 2 users an admin user and a normal user.

Custom Firebase claims were added to identify their roles:

admin -> role = admin
user -> role = user

The Firebase Admin SDK was used during setup to assign these custom claims. The service account file is stored outside the repository and is never committed.
When a user signs in, Firebase returns an ID token. ASP.NET Core validates this JWT using the Firebase project ID, issuer, audience and expiration time. The custom role claim is then used directly by the .NET authorization system.

# Authorization

Two policies were created:

- AdminPolicy
- UserPolicy

AdminPolicy requires the admin role.
UserPolicy accepts both user and admin, because admins should still be able to perform normal read operations.

The normal user has read-only access to products, suppliers, the inventory dashboard and allowed files. Admin users can also create, update and delete warehouse data, adjust stock and manage files.

This was tested with the expected HTTP behavior:

- No valid token -> 401 Unauthorized
- Valid user without enough permission -> 403 Forbidden

I also added a new endpoint
GET /api/auth/me
It returns the Firebase UID, email and role from the validated token. I used this mainly to confirm that Firebase authentication was correctly connected to the ASP.NET Core authentication system.


# Swagger Authentication

Swagger was configured to support Bearer authentication.
After signing in through Firebase and receiving an ID token, the token can be added using the authorize button. Swagger then sends it in the Authorization header when calling protected endpoints.


# MinIO Setup

The MinIO API is available on port 9000 and the MinIO Console on port 9001. The project uses a private bucket called:
warehouse-assets

A Docker volume is also used so uploaded files remain available even if the MinIO container is stopped or recreated.
to keep the AccessKey and SecretKey safe they are stored using .NET User Secrets instead of being committed to the repository. Docker credentials are also read from a local .env file which is ignored by Git.


# Storage Architecture

I created an IFileStorageService interface inside the Application layer.
The interface contains the file operations that the application needs, such as upload, download and delete. The actual MinIO implementation is inside the Infrastructure layer in MinioStorageService.
This means the Application layer does not depend directly on MinIO. It only knows that a file storage service exists. MinIO-specific code such as the MinIO client, bucket operations and object arguments stays inside Infrastructure.



# Product Images

The ProductImage Domain entity still contains the file name and a generic FilePath. I did not add MinIO-specific properties to the Domain because the Domain layer should describe the warehouse business model and should not depend on a specific storage provider.


The actual image is stored in MinIO, while PostgreSQL stores the ProductImage metadata.

Product images support:
- Upload
- List
- Download
- Replace
- Delete

JPEG and PNG images are accepted with a maximum size of 5 MB.
Normal users can list and download images, while upload, replace and delete operations require AdminPolicy.


# Supplier Documents

I added Supplier documents as the second file type stored in MinIO.
A new SupplierDocument Domain entity and ISupplierDocumentRepository were created. The EF Core implementation is inside Infrastructure in SupplierDocumentRepository.

A new EF Core migration was created to add the SupplierDocuments table with a foreign key to Suppliers.
Only PDF documents are accepted, with a maximum size of 10 MB and aupplier documents support upload, list, download, replace and delete operations. Obviously users only have read endpoints and admins have access to all.



# New File Endpoints

Product Images:
- POST /api/products/{id}/image
- GET /api/products/{points

Product Images:
- POST /api/products/{id}/image
- GET /api/products/{id}/images
- GET /api/products/images/{imageId}/download
- PUT /api/products/images/{imageId}
- DELETE /api/products/images/{imageId}

Supplier Documents:
- POST /api/suppliers/{id}/documents
- GET /api/suppliers/{id}/documents
- GET /api/suppliers/documents/{documentId}/download
- PUT /api/suppliers/documents/{documentId}
- DELETE /api/suppliers/documents/{documentId}

Authentication:
- GET /api/auth/me


# Run instructions

Make sure Docker Desktop is running.
Start PostgreSQL and Redis if they are stopped.

Start MinIO:
docker compose up -d
Apply the latest migration:
dotnet ef database update --project Warehouse.Infrastructure --startup-project Warehouse.Presentation --context WarehouseDbContext


Then run:
dotnet restore
dotnet build WarehouseManagement.sln
dotnet run --project Warehouse.Presentation

Swagger: localhost:5035
MinIO Console: localhost:9001

# Screenshots
In the PR description
