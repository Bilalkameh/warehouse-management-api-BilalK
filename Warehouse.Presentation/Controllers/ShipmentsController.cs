using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Warehouse.Application.Commands.Shipments.AssignProductToShipment;
using Warehouse.Application.Commands.Shipments.CreateShipment;
using Warehouse.Application.Commands.Shipments.UpdateShipmentStatus;
using Warehouse.Application.Queries.Shipments.GetShipmentById;
using Warehouse.Presentation.Authorization;

namespace Warehouse.Presentation.Controllers;

[ApiController]
[Route("api/shipments")]
[Authorize(Policy = AuthorizationPolicies.User)]
public class ShipmentsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ShipmentsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetShipmentById([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var request = new GetShipmentByIdRequest
        {
            ShipmentId = id
        };

        var shipment = await _mediator.Send(request, cancellationToken);

        return Ok(shipment);
    }

    [HttpPost]
    [Authorize(Policy = AuthorizationPolicies.Admin)]
    public async Task<IActionResult> CreateShipment([FromBody] CreateShipmentRequest request, CancellationToken cancellationToken)
    {
        var shipment = await _mediator.Send(request, cancellationToken);

        return CreatedAtAction(nameof(GetShipmentById), new { id = shipment.Id }, shipment);
    }

    [HttpPost("{id}/products")]
    [Authorize(Policy = AuthorizationPolicies.Admin)]
    public async Task<IActionResult> AssignProduct(
        [FromRoute] Guid id, [FromBody] AssignProductToShipmentRequest request, CancellationToken cancellationToken)
    {
        request.ShipmentId = id;
        var result = await _mediator.Send(request, cancellationToken);

        return Ok(result);
    }

    [HttpPost("{id}/status")]
    [Authorize(Policy = AuthorizationPolicies.Admin)]
    public async Task<IActionResult> UpdateStatus([FromRoute] Guid id, [FromBody] UpdateShipmentStatusRequest request, CancellationToken cancellationToken)
    {
        request.ShipmentId = id;
        var result = await _mediator.Send(request, cancellationToken);

        return Ok(result);
    }
}