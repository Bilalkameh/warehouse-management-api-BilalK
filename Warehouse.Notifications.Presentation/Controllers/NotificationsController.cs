using MediatR;
using Microsoft.AspNetCore.Mvc;
using Warehouse.Notifications.Application.Notifications.Commands.MarkNotificationAsRead;
using Warehouse.Notifications.Application.Notifications.DTOs;
using Warehouse.Notifications.Application.Notifications.Queries.GetNotifications;

namespace Warehouse.Notifications.Presentation.Controllers;

[ApiController]
[Route("api/notifications")]
public class NotificationsController : ControllerBase
{
    private readonly ISender _mediator;

    public NotificationsController(ISender mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<NotificationResponse>>> GetAll(CancellationToken cancellationToken)
    {
        var notifications = await _mediator.Send(new GetNotificationsRequest(), cancellationToken);

        return Ok(notifications);
    }

    [HttpPut("{id:guid}/read")]
    public async Task<IActionResult> MarkAsRead(Guid id, CancellationToken cancellationToken)
    {
        var wasUpdated = await _mediator.Send(new MarkNotificationAsReadRequest(id), cancellationToken);

        if (!wasUpdated)
            return NotFound();

        return NoContent();
    }
}