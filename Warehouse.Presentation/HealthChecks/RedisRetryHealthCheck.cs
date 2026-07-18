using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Warehouse.Presentation.HealthChecks;

public class RedisRetryHealthCheck : IHealthCheck
{
    private const int MaxAttempts = 3;

    private readonly IDistributedCache _cache;
    private readonly ILogger<RedisRetryHealthCheck> _logger;

    public RedisRetryHealthCheck(IDistributedCache cache, ILogger<RedisRetryHealthCheck> logger)
    {
        _cache = cache;
        _logger = logger;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        Exception? lastError = null;

        for (var attempt = 1; attempt <= MaxAttempts; attempt++)
        {
            try
            {
                await _cache.GetAsync("redis-health-check", cancellationToken);

                return HealthCheckResult.Healthy($"Redis responded on attempt {attempt}.");
            }
            catch (Exception error)
            {
                lastError = error;

                _logger.LogWarning("Redis health check attempt {Attempt} of {MaxAttempts} failed", attempt, MaxAttempts);

                if (attempt < MaxAttempts)
                    await Task.Delay(1000, cancellationToken);
            }
        }

        _logger.LogError(lastError, "Redis health check failed after {MaxAttempts} attempts", MaxAttempts);

        return HealthCheckResult.Unhealthy("Redis health check failed after 3 attempts.", lastError);
    }
}