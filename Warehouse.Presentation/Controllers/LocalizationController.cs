using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace Warehouse.Presentation.Controllers;

[ApiController]
[Route("api/localization")]
public class LocalizationController : ControllerBase
{
    private readonly IStringLocalizer<SharedResources> _localizer;

    public LocalizationController(IStringLocalizer<SharedResources> localizer)
    {
        _localizer = localizer;
    }

    // 1. Get a localized response
    [HttpGet("message")]
    public IActionResult GetMessage()
    {
        return Ok(new{ Message = _localizer["WelcomeMessage"].Value });
    }
}