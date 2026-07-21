using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Warehouse.Presentation.Resources;

namespace Warehouse.Presentation.Controllers;

[ApiController]
[Route("api/localization")]
public class LocalizationController : ControllerBase
{


    // 1. Get a localized response
    [HttpGet("message")]
    public IActionResult GetMessage()
    {
        return Ok(new{ Message = SharedResources.WelcomeMessage} );
    }
}