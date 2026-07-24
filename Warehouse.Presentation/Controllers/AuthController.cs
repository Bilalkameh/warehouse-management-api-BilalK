using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Warehouse.Presentation.Controllers;


[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    [Authorize]
    [HttpGet("current-user")]
    public IActionResult GetCurrentUser()
    {
        return Ok(new
        {
            uid = User.FindFirst("sub")?.Value,
            email = User.FindFirst("email")?.Value,
            role = User.FindFirst("role")?.Value,
        });
    }
}