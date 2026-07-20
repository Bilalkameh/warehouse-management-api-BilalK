using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Warehouse.Presentation.Models;

namespace Warehouse.Presentation.Filters;

public class ModelValidationFilter : IActionFilter
{
    public void OnActionExecuting(ActionExecutingContext context)
    {
        if (context.ModelState.IsValid)
            return;

        var errors = context.ModelState.Values
            .SelectMany(value => value.Errors)
            .Select(error => error.ErrorMessage)
            .Where(message => !string.IsNullOrWhiteSpace(message))
            .ToList();

        var message = "The request is invalid.";

        if (errors.Count > 0)
        {
            message = string.Join(" ", errors);
        }

        var response = new ApiErrorResponse
        {
            Code = "VALIDATION_ERROR",
            Message = message,
            TraceId = context.HttpContext.TraceIdentifier
        };

        context.Result = new BadRequestObjectResult(response);
    }

    public void OnActionExecuted(ActionExecutedContext context)
    {
    }
}