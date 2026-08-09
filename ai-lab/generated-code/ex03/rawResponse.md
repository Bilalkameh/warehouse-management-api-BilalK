No production change is needed. Create:

tests/Warehouse.Api.UnitTests/Middleware/ExceptionHandlingMiddlewareTests.cs


using System.Text.Json;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using Warehouse.Application.Exceptions;
using Warehouse.Domain.Exceptions;
using Warehouse.Presentation.Middleware;
using Warehouse.Presentation.Models;

namespace Warehouse.Api.UnitTests.Middleware;

public class ExceptionHandlingMiddlewareTests
{
private const string TraceId = "test-trace-id";

    [Fact]
    public async Task InvokeAsync_NextCompletesSuccessfully_CallsNextOnceAndDoesNotLogError()
    {
        var context = CreateHttpContext();
        var nextMock = new Mock<RequestDelegate>();
        var loggerMock = new Mock<ILogger<ExceptionHandlingMiddleware>>();

        nextMock
            .Setup(next => next(It.IsAny<HttpContext>()))
            .Callback<HttpContext>(currentContext =>
                currentContext.Response.StatusCode =
                    StatusCodes.Status204NoContent)
            .Returns(Task.CompletedTask);

        var middleware = new ExceptionHandlingMiddleware(
            nextMock.Object,
            loggerMock.Object);

        await middleware.InvokeAsync(context);

        context.Response.StatusCode.Should()
            .Be(StatusCodes.Status204NoContent);

        context.Response.Body.Length.Should().Be(0);

        nextMock.Verify(
            next => next(context),
            Times.Once);

        loggerMock.Verify(
            logger => logger.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((_, _) => true),
                It.IsAny<Exception?>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Never);
    }

    [Fact]
    public async Task InvokeAsync_NotFoundException_ReturnsNotFoundResponse()
    {
        var exception = new NotFoundException(
            "Product was not found.");

        await InvokeAndAssertExceptionAsync(
            exception,
            StatusCodes.Status404NotFound,
            "NOT_FOUND",
            exception.Message);
    }

    [Fact]
    public async Task InvokeAsync_BadRequestException_ReturnsBadRequestResponse()
    {
        var exception = new BadRequestException(
            "The request is invalid.");

        await InvokeAndAssertExceptionAsync(
            exception,
            StatusCodes.Status400BadRequest,
            "BAD_REQUEST",
            exception.Message);
    }

    [Fact]
    public async Task InvokeAsync_ConflictException_ReturnsConflictResponse()
    {
        var exception = new ConflictException(
            "A product with this SKU already exists.");

        await InvokeAndAssertExceptionAsync(
            exception,
            StatusCodes.Status409Conflict,
            "CONFLICT",
            exception.Message);
    }

    [Fact]
    public async Task InvokeAsync_ValidationException_ReturnsValidationErrorResponse()
    {
        var exception = new ValidationException(new[]
        {
            new ValidationFailure(
                "Name",
                "Product name is required.")
        });

        await InvokeAndAssertExceptionAsync(
            exception,
            StatusCodes.Status400BadRequest,
            "VALIDATION_ERROR",
            "Product name is required.");
    }

    [Fact]
    public async Task InvokeAsync_BusinessRuleException_ReturnsBusinessRuleErrorResponse()
    {
        var exception = new BusinessRuleException(
            "Archived products cannot be updated.");

        await InvokeAndAssertExceptionAsync(
            exception,
            StatusCodes.Status400BadRequest,
            "BUSINESS_RULE_ERROR",
            exception.Message);
    }

    [Fact]
    public async Task InvokeAsync_UnexpectedException_ReturnsSafeInternalServerErrorResponse()
    {
        var exception = new IOException(
            "The database connection failed with sensitive internal details.");

        var response = await InvokeAndAssertExceptionAsync(
            exception,
            StatusCodes.Status500InternalServerError,
            "INTERNAL_SERVER_ERROR",
            "An unexpected error occurred.");

        response.Message.Should().NotContain(exception.Message);
    }

    [Fact]
    public async Task InvokeAsync_ValidationExceptionWithNoErrors_ReturnsDefaultValidationMessage()
    {
        var exception = new ValidationException(
            Array.Empty<ValidationFailure>());

        await InvokeAndAssertExceptionAsync(
            exception,
            StatusCodes.Status400BadRequest,
            "VALIDATION_ERROR",
            "The request is invalid.");
    }

    [Fact]
    public async Task InvokeAsync_ValidationExceptionWithMultipleErrors_ReturnsFirstValidationMessage()
    {
        var exception = new ValidationException(new[]
        {
            new ValidationFailure(
                "Name",
                "Product name is required."),
            new ValidationFailure(
                "Price",
                "Product price must be greater than zero.")
        });

        await InvokeAndAssertExceptionAsync(
            exception,
            StatusCodes.Status400BadRequest,
            "VALIDATION_ERROR",
            "Product name is required.");
    }

    private static DefaultHttpContext CreateHttpContext()
    {
        var context = new DefaultHttpContext
        {
            TraceIdentifier = TraceId
        };

        context.Response.Body = new MemoryStream();

        return context;
    }

    private static async Task<ApiErrorResponse> InvokeAndAssertExceptionAsync(
        Exception exception,
        int expectedStatusCode,
        string expectedCode,
        string expectedMessage)
    {
        var context = CreateHttpContext();
        var nextMock = new Mock<RequestDelegate>();
        var loggerMock =
            new Mock<ILogger<ExceptionHandlingMiddleware>>();

        nextMock
            .Setup(next => next(It.IsAny<HttpContext>()))
            .ThrowsAsync(exception);

        var middleware = new ExceptionHandlingMiddleware(
            nextMock.Object,
            loggerMock.Object);

        await middleware.InvokeAsync(context);

        context.Response.StatusCode.Should()
            .Be(expectedStatusCode);

        context.Response.ContentType.Should()
            .StartWith("application/json");

        context.Response.Body.Position = 0;

        var response =
            await JsonSerializer.DeserializeAsync<ApiErrorResponse>(
                context.Response.Body,
                new JsonSerializerOptions(JsonSerializerDefaults.Web));

        response.Should().NotBeNull();
        response!.Code.Should().Be(expectedCode);
        response.Message.Should().Be(expectedMessage);
        response.TraceId.Should().Be(TraceId);

        nextMock.Verify(
            next => next(context),
            Times.Once);

        VerifyErrorLogged(loggerMock, exception);

        return response;
    }

    private static void VerifyErrorLogged(
        Mock<ILogger<ExceptionHandlingMiddleware>> loggerMock,
        Exception exception)
    {
        loggerMock.Verify(
            logger => logger.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((state, _) =>
                    state.ToString() ==
                    "An unhandled exception occurred."),
                It.Is<Exception?>(loggedException =>
                    ReferenceEquals(loggedException, exception)),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }
}


This covers every mapping:

Exception	Status	Code
NotFoundException	404	NOT_FOUND
BadRequestException	400	BAD_REQUEST
ConflictException	409	CONFLICT
ValidationException	400	VALIDATION_ERROR
BusinessRuleException	400	BUSINESS_RULE_ERROR
Unexpected exception	500	INTERNAL_SERVER_ERROR