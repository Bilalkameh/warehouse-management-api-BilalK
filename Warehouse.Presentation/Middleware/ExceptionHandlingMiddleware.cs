using FluentValidation;
using Warehouse.Application.Exceptions;
using Warehouse.Domain.Exceptions;
using Warehouse.Presentation.Models;

namespace Warehouse.Presentation.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception,"An unhandled exception occurred.");

            await HandleExceptionAsync(context, exception);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        int statusCode;
        string code;
        string message;

        if (exception is NotFoundException)
        {
            statusCode = StatusCodes.Status404NotFound;
            code = "NOT_FOUND";
            message = exception.Message;
        }
        else if (exception is ValidationException validationException)
        {
            statusCode = StatusCodes.Status400BadRequest;
            code = "VALIDATION_ERROR";

            message = validationException.Errors
                .FirstOrDefault()?.ErrorMessage
                ?? "The request is invalid.";
        }
        else if (exception is BusinessRuleException)
        {
            statusCode = StatusCodes.Status400BadRequest;
            code = "BUSINESS_RULE_ERROR";
            message = exception.Message;
        }
        else
        {
            statusCode = StatusCodes.Status500InternalServerError;
            code = "INTERNAL_SERVER_ERROR";
            message = "An unexpected error occurred.";
        }

        var response = new ApiErrorResponse
        {
            Code = code,
            Message = message,
            TraceId = context.TraceIdentifier
        };

        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/json";
        await context.Response.WriteAsJsonAsync(response);
    }
}