using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using Warehouse.Presentation.Middleware;

namespace Warehouse.Api.UnitTests.Middleware;

public class RequestTimingMiddlewareTests
{
    [Fact]
    public async Task InvokeAsync_SlowRequest_LogsPathStatusCodeAndElapsedMilliseconds()
    {
        var loggerMock =
            new Mock<ILogger<RequestTimingMiddleware>>();

        var context = new DefaultHttpContext();
        context.Request.Method = HttpMethods.Post;
        context.Request.Path = "/api/products";

        RequestDelegate next = async currentContext =>
        {
            currentContext.Response.StatusCode =
                StatusCodes.Status201Created;

            await Task.Delay(600);
        };

        var middleware = new RequestTimingMiddleware(next, loggerMock.Object);

        await middleware.InvokeAsync(context);

        loggerMock.Verify(
            logger => logger.Log(LogLevel.Warning,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((state, _) => IsExpectedLogMessage(state)),
                It.IsAny<Exception?>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }
    
    private static bool IsExpectedLogMessage(object state)
    {
        var message = state.ToString();

        return message != null && message.Contains("POST /api/products") 
                               && message.Contains("returned 201") 
                               && Regex.IsMatch(message, @"took \d+ ms");
    }
}