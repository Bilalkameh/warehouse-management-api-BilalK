using Microsoft.AspNetCore.Http;
using Warehouse.Application.Interfaces;

namespace Warehouse.Presentation.Services;

public class HttpContextCorrelationIdAccessor : ICorrelationIdAccessor
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public HttpContextCorrelationIdAccessor(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public string CorrelationId => _httpContextAccessor.HttpContext?.TraceIdentifier
                                   ?? throw new InvalidOperationException("No active HTTP request.");
}