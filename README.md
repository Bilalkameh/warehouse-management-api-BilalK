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


To run:
dotnet restore
dotnet build
dotnet run
Then open: https://localhost:<port>/swagger
After this all endpoints can be tested 

to run these the path should be :
warehouse-management-api-BilalK\warehouse-management\warehouse-management>


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

 - Suppliers:
 GET     /api/suppliers -> Get all suppliers
 GET     /api/suppliers/{id} -> Get supplier by ID
 POST    /api/suppliers -> Create supplier
 DELETE  /api/suppliers/{id} -> Deactivate supplier



