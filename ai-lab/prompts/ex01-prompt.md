Context: We are building new features over the warehouse API structure. We need to add a new feature.The warehouse system requires a fresh management endpoint to prevent food/chemical spoilage

Task: Create a new GET /api/products/expiring-soon , which must reliably fetch and return all product entities scheduled to expire within the next 30 calendar days

Requirements:
The absolute controller action methods and route bindings.
The application-level service logic queries filtering by expiration timestamps.
Fully validated request parameter shapes and Data Transfer Objects (DTOs).
Accompanying unit tests and mock boundaries.

The unit tests have to go in the tests folder. Make sure to conserve the clean DDD structure