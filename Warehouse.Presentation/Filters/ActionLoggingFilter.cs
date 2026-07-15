using Microsoft.AspNetCore.Mvc.Filters;

namespace Warehouse.Presentation.Filters;

public class ActionLoggingFilter : IActionFilter
{
    private readonly ILogger<ActionLoggingFilter> _logger;

    public ActionLoggingFilter(ILogger<ActionLoggingFilter> logger)
    {
        _logger = logger;
    }

    public void OnActionExecuting(ActionExecutingContext context)
    {
        var controller = context.RouteData.Values["controller"];
        var action = context.RouteData.Values["action"];

        _logger.LogInformation("Executing {Controller}.{Action}", controller, action);
    }

    public void OnActionExecuted(ActionExecutedContext context)
    {
        var controller = context.RouteData.Values["controller"];
        var action = context.RouteData.Values["action"];

        _logger.LogInformation("Executed {Controller}.{Action}", controller, action);
    }
}