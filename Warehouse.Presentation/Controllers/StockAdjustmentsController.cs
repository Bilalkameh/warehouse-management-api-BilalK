using MediatR;
using Microsoft.AspNetCore.Mvc;
using Warehouse.Application.Commands.Stock;
using Microsoft.AspNetCore.Authorization;
using Warehouse.Presentation.Authorization;

namespace Warehouse.Presentation.Controllers;

[ApiController]
[Route("api/stock-adjustments")]
[Authorize(Policy = AuthorizationPolicies.Admin)]
public class StockAdjustmentsController : ControllerBase
{
    private readonly IMediator _mediator;

    public StockAdjustmentsController(IMediator mediator)
    {
        _mediator = mediator;
    }


    // 1. Adjust product stock
    [HttpPost]
    public async Task<IActionResult> AdjustStock([FromBody] StockAdjustmentRequest request, CancellationToken cancellationToken)
    {
        var product = await _mediator.Send( request, cancellationToken);

        return Ok(product);
    }
}