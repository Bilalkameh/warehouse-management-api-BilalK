Implemented and verified the endpoint in warehouse-out-of-stock-source.zip.

Implemented flow

ProductsController → MediatR query → query handler → IProductRepository → EF Core

The repository performs the filtering before ToListAsync, includes supplier data for the existing mapping, uses AsNoTracking, and propagates the cancellation token.

Endpoint contract
Property	Behavior
Method	GET /api/products/out-of-stock
Authorization	Existing User policy
Filter	QuantityInStock == 0 && IsArchived == false
Success	200 OK with ProductViewModel[]
No matches	200 OK with []
Unauthenticated	401 Unauthorized
Files created
GetOutOfStockProductsRequest.cs
GetOutOfStockProductsHandler.cs
GetOutOfStockProductsHandlerTests.cs
OutOfStockProductsEndpointTests.cs
Files modified
IProductRepository.cs
ProductRepository.cs
ProductsController.cs
TestAuthHandler.cs
CustomWebApplicationFactory.cs

The factory now supplies inert test MinIO settings before replacing storage with the existing fake. No external service is contacted.

Tests added

Five handler tests cover:

Returning out-of-stock products.
Excluding in-stock products.
Excluding archived products.
Returning an empty collection.
Passing the cancellation token.

Three HTTP integration tests cover:

200 OK and deserialization into List<ProductViewModel>.
Exact seeded IDs and representative fields.
Exclusion of in-stock and archived products.
Empty-array behavior.
401 Unauthorized.
Database reset before every test.
Verification results
Build: succeeded, 0 errors.
Existing warning: one unrelated CS8602 in ProductsEndpointTests.cs.
Unit tests: 31/31 passed.
Integration tests: 14/14 passed.
Focused handler tests: 5/5 passed.
Focused endpoint tests: 3/3 passed.
Total: 45 passed, 0 failed.

The standard command was also attempted:

dotnet test ./WarehouseManagement.sln \
--no-restore \
--no-build \
-m:1 \
--logger 'console;verbosity=minimal'

This container’s VSTest 17.11 host crashes before discovery against the project’s Microsoft.NET.Test.Sdk 18.0.0. Therefore, the same final assemblies were executed with the project’s xUnit 2.9 runner:

dotnet exec \
--runtimeconfig ./tests/Warehouse.Api.UnitTests/bin/Debug/net8.0/Warehouse.Api.UnitTests.runtimeconfig.json \
--depsfile ./tests/Warehouse.Api.UnitTests/bin/Debug/net8.0/Warehouse.Api.UnitTests.deps.json \
./tests/Warehouse.Api.IntegrationTests/bin/Debug/net8.0/xunit-runner-temp/xunit.console.dll \
./tests/Warehouse.Api.UnitTests/bin/Debug/net8.0/Warehouse.Api.UnitTests.dll \
-nologo -nocolor -parallel none
dotnet exec \
--runtimeconfig ./tests/Warehouse.Api.IntegrationTests/bin/Debug/net8.0/Warehouse.Api.IntegrationTests.runtimeconfig.json \
--depsfile ./tests/Warehouse.Api.IntegrationTests/bin/Debug/net8.0/Warehouse.Api.IntegrationTests.deps.json \
./tests/Warehouse.Api.IntegrationTests/bin/Debug/net8.0/xunit-runner-temp/xunit.console.dll \
./tests/Warehouse.Api.IntegrationTests/bin/Debug/net8.0/Warehouse.Api.IntegrationTests.dll \
-nologo -nocolor -parallel none

It generated these 9 files:
GetOutOfStockProductsRequest.cs
Code
GetOutOfStockProductsHandler.cs
Code
GetOutOfStockProductsHandlerTests.cs
Code
OutOfStockProductsEndpointTests.cs
Code
IProductRepository.cs
Code
ProductRepository.cs
Code
ProductsController.cs
Code
TestAuthHandler.cs
Code
CustomWebApplicationFactory.cs
Code