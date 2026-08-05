The controller already uses the api/products base route and sends product queries through MediatR, so the new action should follow the same pattern as low-stock.

1. Add this using
   using Warehouse.Application.Queries.Products.GetOutOfStockProducts;
2. Add this endpoint before the final } of ProductsController
   // 18. Get out-of-stock products

[HttpGet("out-of-stock")]
[ProducesResponseType(typeof(List<ProductViewModel>), StatusCodes.Status200OK)]
public async Task<IActionResult> GetOutOfStockProducts(
CancellationToken cancellationToken)
{
var products = await _mediator.Send(
new GetOutOfStockProductsRequest(),
cancellationToken);

    return Ok(products);
}

This creates:

GET /api/products/out-of-stock

The endpoint inherits the controller’s existing AuthorizationPolicies.User policy, so no additional [Authorize] attribute is needed.

The corresponding query handler should return products matching:

product.QuantityInStock == 0 && !product.IsArchived

and should return an empty list with 200 OK when none exist.