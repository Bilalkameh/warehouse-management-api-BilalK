Context: The Warehouse API already contains an integration test project using CustomWebApplicationFactor and a seeded in-memory database.

The current ProductsEndpointTests contains basic product tests but does not fully test the product creation, image upload, and deletion flows.



Task: Generate complete database-backed integration tests for the following routes:

- POST /api/products (Full product initialization flow)

- POST /api/products/{id}/image (Binary image stream processing)

- DELETE /api/products/{id} (Resource teardown and cascading cleanup rules)



Requirements:

- Write assertions verifying HTTP response headers/status codes

- Model property match conditions

- Use persistent server side-effects

- Verify that a created product can be retrieved afterward with the correct values.



Constraints: No new packages, production services, or architectural abstractions.