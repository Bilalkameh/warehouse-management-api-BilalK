Context: The Warehouse API currently contains an active custom HTTP middleware named ExceptionHandlingMiddleware in the Presentation layer.

The middleware maps application, validation, domain, and unexpected exceptions to consistent HTTP error responses.
RequestTimingMiddleware already has an isolated unit test, but ExceptionHandlingMiddleware does not currently have complete unit test coverage.

ExceptionHandlingMiddleware currently contains:
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
        
        else if (exception is BadRequestException)
        {
            statusCode = StatusCodes.Status400BadRequest;
            code = "BAD_REQUEST";
            message = exception.Message;
        }

        else if (exception is ConflictException)
        {
            statusCode = StatusCodes.Status409Conflict;
            code = "CONFLICT";
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

Task: Generate a complete isolated unit test suite for ExceptionHandlingMiddleware covering positive tests, negative tests, and relevant edge cases.


Requirements:
-Positive tests: Verify the standard execution path when the next middleware completes successfully.
-Negative tests: Verify missing database entities, bad requests, validation failures, business-rule failures, conflicts, and unexpected infrastructure failures.
-Include edge cases relevant to this middleware, such as an empty validation error collection and multiple validation errors.
-Verify the correct HTTP status code and error code for every supported exception type.
-Verify that exceptions are logged and that the next middleware delegate is called exactly once.
-Use the existing xUnit, Moq, and FluentAssertions packages.


Constraints:
-Do not introduce new packages, production services, or architectural abstractions.
-Do not modify ExceptionHandlingMiddleware unless a production defect is discovered and clearly explained. 


