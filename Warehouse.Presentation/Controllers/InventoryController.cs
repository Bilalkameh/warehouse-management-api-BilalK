using MediatR;
using Microsoft.AspNetCore.Mvc;
using Warehouse.Application.Queries.Inventory.GetInventoryDashboard;

namespace Warehouse.Presentation.Controllers;

//Controller
[ApiController]
[Route("api/inventory")]
public class InventoryController : ControllerBase
{
    private readonly IMediator _mediator;

    public InventoryController(IMediator mediator)
    {
        _mediator = mediator;
    }


    // 1. Get inventory dashboard
    [HttpGet("dashboard")]
    public async Task<IActionResult> GetDashboard(CancellationToken cancellationToken)
    {
        var request = new GetInventoryDashboardRequest();

        var dashboard = await _mediator.Send(request, cancellationToken);

        return Ok(dashboard);
    }
}